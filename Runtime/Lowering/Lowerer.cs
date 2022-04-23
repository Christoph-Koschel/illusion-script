using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int labelCount;

        private Lowerer()
        {
            labelCount = 0;
        }

        private BoundLabel GenerateLabel()
        {
            string name = $"l" + labelCount;
            labelCount++;
            return new BoundLabel(name);
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new Lowerer();
            BoundStatement result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            ImmutableArray<BoundStatement>.Builder builder = ImmutableArray.CreateBuilder<BoundStatement>();
            Stack<BoundStatement> stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                BoundStatement current = stack.Pop();
                if (current is BoundBlockStatement block)
                {
                    foreach (BoundStatement blockStatement in block.statements.Reverse())
                    {
                        stack.Push(blockStatement);
                    }
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.elseBody == null)
            {
                /*
                 * Before:
                 * if (<condition>)
                 * {
                 *      <body>
                 * }
                 *
                 * After:
                 * gotoIfFalse <condition> end
                 * <body>
                 * end:
                 */
                BoundLabel endBoundLabel = GenerateLabel();

                BoundConditionalGotoStatement gotoEnd =
                    new BoundConditionalGotoStatement(endBoundLabel, node.condition, false);
                BoundLabelStatement endLabelStatement = new BoundLabelStatement(endBoundLabel);
                BoundBlockStatement result =
                    new BoundBlockStatement(
                        ImmutableArray.Create<BoundStatement>(gotoEnd, node.body, endLabelStatement));
                return RewriteStatement(result);
            }
            else
            {
                /*
                * Before:
                * if (<condition>)
                * {
                *      <body @1>
                * } else {
                *      <body @2>
                * }
                *
                * After:
                * gotoIfFalse <condition> else
                * <body @1>
                * goto end
                * else:
                * <body @2>
                * end:
                */

                BoundLabel elseBoundLabel = GenerateLabel();
                BoundLabel endBoundLabel = GenerateLabel();

                BoundConditionalGotoStatement gotoFalse =
                    new BoundConditionalGotoStatement(elseBoundLabel, node.condition, false);
                BoundGotoStatement gotoEnd =
                    new BoundGotoStatement(endBoundLabel);
                BoundLabelStatement elseLabelStatement = new BoundLabelStatement(elseBoundLabel);
                BoundLabelStatement endLabelStatement = new BoundLabelStatement(endBoundLabel);

                BoundBlockStatement result =
                    new BoundBlockStatement(
                        ImmutableArray.Create<BoundStatement>(gotoFalse,
                            node.body,
                            gotoEnd,
                            elseLabelStatement,
                            node.elseBody,
                            endLabelStatement
                        ));
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            /*
             * Before:
             * while (<condition>)
             * {
             *      <body>
             * }
             *
             * After:
             * goto check
             * continue:
             * <body>
             * check:
             * gotoTrue <condition> continue 
             * end:
             * 
             */

            BoundLabel endBoundLabel = GenerateLabel();
            BoundLabel checkBoundLabel = GenerateLabel();
            BoundLabel continueBoundLabel = GenerateLabel();

            BoundLabelStatement endLabelStatement = new BoundLabelStatement(endBoundLabel);
            BoundLabelStatement checkLabelStatement = new BoundLabelStatement(checkBoundLabel);
            BoundLabelStatement continueLabelStatement = new BoundLabelStatement(continueBoundLabel);

            BoundGotoStatement gotoCheck = new BoundGotoStatement(checkBoundLabel);
            BoundConditionalGotoStatement goToTrue = new BoundConditionalGotoStatement(continueBoundLabel, node.condition);

            BoundBlockStatement result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                gotoCheck,
                continueLabelStatement,
                node.body,
                checkLabelStatement,
                goToTrue,
                endLabelStatement
            ));

            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            /*
             * Before:
             * for (i = <start> to <end>) { <body> }
             *
             * After:
             * {
             *      let i = <start>
             *      let end = <end>
             *      while(i < end)
             *      {
             *          {
             *              <body>
             *          }
             *          i = i + 1;
             *      }
             * }
             */

            BoundVariableDeclarationStatement variableDeclaration =
                new BoundVariableDeclarationStatement(node.variable, node.startExpression);
            BoundVariableExpression variableExpression = new BoundVariableExpression(node.variable);
            VariableSymbol endBoundSymbol = new VariableSymbol("__while__end__", true, typeof(int));
            BoundVariableDeclarationStatement endDeclaration = new BoundVariableDeclarationStatement(endBoundSymbol, node.endExpression);
            BoundBinaryExpression condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxType.LessToken, typeof(int), typeof(int)),
                new BoundVariableExpression(endBoundSymbol)
            );

            BoundExpressionStatement increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxType.PlusToken, typeof(int), typeof(int)),
                        new BoundLiteralExpression(1)
                    )
                )
            );

            BoundBlockStatement whileBlock =
                new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.body, increment));
            BoundWhileStatement whileStatement = new BoundWhileStatement(condition, whileBlock);
            BoundBlockStatement result =
                new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    variableDeclaration,
                    endDeclaration,
                    whileStatement
                ));

            return RewriteStatement(result);
        }
    }
}