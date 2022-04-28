using System;
using System.CodeDom.Compiler;
using System.ComponentModel.Design.Serialization;
using System.IO;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Binding
{
    internal static class BoundNodePrinter
    {
        public static void WriteTo(this BoundNode node, TextWriter writer)
        {
            if (writer is IndentedTextWriter iw)
            {
                WriteTo(node, iw);
            }
            else
            {
                WriteTo(node, new IndentedTextWriter(writer));
            }
        }

        public static void WriteTo(this BoundNode node, IndentedTextWriter writer)
        {
            switch (node.boundType)
            {
                case BoundNodeType.UnaryExpression:
                    WriteUnaryExpression((BoundUnaryExpression)node, writer);
                    break;
                case BoundNodeType.LiteralExpression:
                    WriteLiteralExpression((BoundLiteralExpression)node, writer);
                    break;
                case BoundNodeType.BinaryExpression:
                    WriteBinaryExpression((BoundBinaryExpression)node, writer);
                    break;
                case BoundNodeType.VariableExpression:
                    WriteVariableExpression((BoundVariableExpression)node, writer);
                    break;
                case BoundNodeType.AssignmentExpression:
                    WriteAssignmentExpression((BoundAssignmentExpression)node, writer);
                    break;
                case BoundNodeType.CallExpression:
                    WriteCallExpression((BoundCallExpression)node, writer);
                    break;
                case BoundNodeType.ConversionExpression:
                    WriteConversionExpression((BoundConversionExpression)node, writer);
                    break;
                case BoundNodeType.ErrorExpression:
                    WriteErrorExpression((BoundErrorExpression)node, writer);
                    break;
                case BoundNodeType.BlockStatement:
                    WriteBlockStatement((BoundBlockStatement)node, writer);
                    break;
                case BoundNodeType.ExpressionStatement:
                    WriteExpressionStatement((BoundExpressionStatement)node, writer);
                    break;
                case BoundNodeType.VariableDeclarationStatement:
                    WriteVariableDeclarationStatement((BoundVariableDeclarationStatement)node, writer);
                    break;
                case BoundNodeType.IfStatement:
                    WriteIfStatement((BoundIfStatement)node, writer);
                    break;
                case BoundNodeType.WhileStatement:
                    WriteWhileStatement((BoundWhileStatement)node, writer);
                    break;
                case BoundNodeType.DoWhileStatement:
                    WriteDoWhileStatement((BoundDoWhileStatement)node, writer);
                    break;
                case BoundNodeType.ForStatement:
                    WriteForStatement((BoundForStatement)node, writer);
                    break;
                case BoundNodeType.GotoStatement:
                    WriteGotoStatement((BoundGotoStatement)node, writer);
                    break;
                case BoundNodeType.LabelStatement:
                    WriteLabelStatement((BoundLabelStatement)node, writer);
                    break;
                case BoundNodeType.ConditionalGotoStatement:
                    WriteConditionalGotoStatement((BoundConditionalGotoStatement)node, writer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void WriteNestedStatement(this IndentedTextWriter writer, BoundStatement node)
        {
            bool needsIndentation = !(node is BoundBlockStatement block);
            if (needsIndentation)
            {
                writer.Indent++;
            }

            node.WriteTo(writer);

            if (needsIndentation)
            {
                writer.Indent--;
            }
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence,
            BoundExpression expression)
        {
            if (expression is BoundUnaryExpression unary)
            {
                writer.WriteNestedExpression(parentPrecedence,
                    SyntaxFacts.GetUnaryOperatorPrecedence(unary.unaryOperator.type), unary);
            }
            else if (expression is BoundBinaryExpression binary)
            {
                writer.WriteNestedExpression(parentPrecedence,
                    SyntaxFacts.GetUnaryOperatorPrecedence(binary.binaryOperator.type), binary);
            }
            else
            {
                expression.WriteTo(writer);
            }
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, int parent, int current,
            BoundExpression expression)
        {
            var needsParens = parent <= current;
            if (needsParens)
            {
                writer.WritePunctuation("(");
            }

            expression.WriteTo(writer);

            if (needsParens)
            {
                writer.WritePunctuation(")");
            }
        }

        private static void WriteUnaryExpression(BoundUnaryExpression node, IndentedTextWriter writer)
        {
            var precedence = SyntaxFacts.GetUnaryOperatorPrecedence(node.unaryOperator.type);
            var op = SyntaxFacts.GetText(node.unaryOperator.type);
            writer.WritePunctuation(op);

            writer.WriteNestedExpression(precedence, node.right);
        }

        private static void WriteLiteralExpression(BoundLiteralExpression node, IndentedTextWriter writer)
        {
            if (node.type == TypeSymbol.Int)
            {
                writer.WriteNumber(node.value.ToString());
            }
            else if (node.type == TypeSymbol.Bool)
            {
                writer.WriteKeyword(node.value.ToString());
            }
            else if (node.type == TypeSymbol.String)
            {
                writer.WriteString('"' + node.value.ToString() + '"');
            }
            else
            {
                throw new Exception($"unexpected type {node.type}");
            }
        }

        private static void WriteBinaryExpression(BoundBinaryExpression node, IndentedTextWriter writer)
        {
            var precedence = SyntaxFacts.GetBinaryOperatorPrecedence(node.binaryOperator.type);
            var op = SyntaxFacts.GetText(node.binaryOperator.type);

            writer.WriteNestedExpression(precedence, node.left);
            writer.WritePunctuation(op);
            writer.WriteNestedExpression(precedence, node.right);
        }

        private static void WriteVariableExpression(BoundVariableExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.variableSymbol.name);
        }

        private static void WriteAssignmentExpression(BoundAssignmentExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.variableSymbol.name);
            writer.WritePunctuation(" = ");
            node.expression.WriteTo(writer);
        }

        private static void WriteCallExpression(BoundCallExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.function.name);
            writer.WritePunctuation("(");

            var isFirst = true;
            foreach (BoundExpression argument in node.arguments)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WritePunctuation(", ");
                }

                argument.WriteTo(writer);
            }

            writer.WritePunctuation(")");
        }

        private static void WriteConversionExpression(BoundConversionExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.type.name);
            writer.WritePunctuation("(");
            node.expression.WriteTo(writer);
            writer.WritePunctuation(")");
        }

        private static void WriteErrorExpression(BoundErrorExpression node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("?");
        }

        private static void WriteBlockStatement(BoundBlockStatement node, IndentedTextWriter writer
        )
        {
            writer.WritePunctuation("{");
            writer.WriteLine();
            writer.Indent++;

            foreach (BoundStatement statement in node.statements)
            {
                statement.WriteTo(writer);
            }

            writer.Indent--;
            writer.WritePunctuation("}");
            writer.WriteLine();
        }

        private static void WriteExpressionStatement(BoundExpressionStatement node, IndentedTextWriter writer
        )
        {
            node.expression.WriteTo(writer);
            writer.WriteLine();
        }

        private static void WriteVariableDeclarationStatement(BoundVariableDeclarationStatement node,
            IndentedTextWriter writer)
        {
            writer.WriteKeyword(node.variable.isReadOnly ? "const " : "let ");
            writer.WriteIdentifier(node.variable.name);
            writer.WritePunctuation(" = ");
            node.initializer.WriteTo(writer);
            writer.WriteLine();
        }

        private static void WriteIfStatement(BoundIfStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("if ");
            writer.WritePunctuation("(");
            node.condition.WriteTo(writer);
            writer.WritePunctuation(")");
            writer.WriteLine();
            writer.WriteNestedStatement(node.body);

            if (node.elseBody != null)
            {
                writer.WriteKeyword("else ");
                writer.WriteLine();
                writer.WriteNestedStatement(node.elseBody);
            }
        }

        private static void WriteWhileStatement(BoundWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("while ");
            writer.WritePunctuation("(");
            node.condition.WriteTo(writer);
            writer.WritePunctuation(")");
            writer.WriteLine();
            writer.WriteNestedStatement(node.body);
        }

        private static void WriteDoWhileStatement(BoundDoWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("do ");
            writer.WriteLine();
            writer.WriteNestedStatement(node.body);
            writer.WriteKeyword("while ");
            writer.WritePunctuation("(");
            node.condition.WriteTo(writer);
            writer.WritePunctuation(")");
            writer.WriteLine();
        }

        private static void WriteForStatement(BoundForStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("for ");
            writer.WritePunctuation("(");
            writer.WriteIdentifier(node.variable.name);
            writer.WritePunctuation(" = ");
            node.startExpression.WriteTo(writer);
            writer.WriteKeyword(" to ");
            node.endExpression.WriteTo(writer);
            writer.WritePunctuation(")");
            writer.WriteNestedStatement(node.body);
        }

        private static void WriteGotoStatement(BoundGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto ");
            writer.WriteIdentifier(node.BoundLabel.name);
            writer.WriteLine();
        }

        private static void WriteLabelStatement(BoundLabelStatement node, IndentedTextWriter writer)
        {
            var unindent = writer.Indent > 0;
            if (unindent)
            {
                writer.Indent--;
            }
            writer.WritePunctuation(node.BoundLabel.name);
            writer.WritePunctuation(":");
            writer.WriteLine();
            
            if (unindent)
            {
                writer.Indent++;
            }
        }

        private static void WriteConditionalGotoStatement(BoundConditionalGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto ");
            writer.WriteIdentifier(node.boundLabel.name);
            writer.WriteKeyword(node.jmpIfTrue ? " if " : " unless ");
            node.condition.WriteTo(writer);
            writer.WriteLine();
        }
    }
}