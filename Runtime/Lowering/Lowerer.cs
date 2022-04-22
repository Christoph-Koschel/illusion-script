using System.Collections.Immutable;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter

    {
        private Lowerer()
        {
        }

        public static BoundStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            /*
             * Before:
             * for (i = 0 to 10) { <body> }
             *
             * After:
             * {
             *      let i = 0
             *      while(i < 10)
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
            BoundBinaryExpression condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxType.LessToken, typeof(int), typeof(int)),
                node.endExpression);

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
                new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));

            return RewriteStatement(result);
        }
    }
}