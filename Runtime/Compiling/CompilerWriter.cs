using System;
using System.IO;
using System.Text;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;

namespace IllusionScript.Runtime.Compiling
{
    public abstract class CompilerWriter
    {
        protected readonly StreamWriter writer;

        protected CompilerWriter(FileStream writer)
        {
            this.writer = new StreamWriter(writer, Encoding.UTF8);
        }

        protected void WriteStatement(BoundStatement statement)
        {
            switch (statement.boundType)
            {
                case BoundNodeType.BlockStatement:
                    WriteBlockStatement((BoundBlockStatement)statement);
                    break;
                case BoundNodeType.ExpressionStatement:
                    WriteExpressionStatement((BoundExpressionStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.VariableDeclarationStatement:
                    WriteVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.ReturnStatement:
                    WriteReturnStatement((BoundReturnStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.GotoStatement:
                    WriteGotoStatement((BoundGotoStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.ConditionalGotoStatement:
                    WriteConditionalGotoStatement((BoundConditionalGotoStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.LabelStatement:
                    WriteLabelStatement((BoundLabelStatement)statement);
                    writer.Write("\n");
                    break;
                default:
                    writer.Close(); // TODO delete file
                    throw new Exception($"Undefined statement {statement.boundType}");
            }
        }

        protected abstract void WriteLabelStatement(BoundLabelStatement statement);

        protected abstract void WriteConditionalGotoStatement(BoundConditionalGotoStatement statement);

        protected abstract void WriteGotoStatement(BoundGotoStatement statement);

        protected abstract void WriteReturnStatement(BoundReturnStatement statement);

        protected abstract void WriteVariableDeclarationStatement(BoundVariableDeclarationStatement statement);

        protected abstract void WriteExpressionStatement(BoundExpressionStatement statement);

        protected abstract void WriteBlockStatement(BoundBlockStatement statement);
        
        protected void WriteExpression(BoundExpression expression)
        {
            switch (expression.boundType)
            {
                case BoundNodeType.ConversionExpression:
                    WriteConversionExpression((BoundConversionExpression)expression);
                    break;
                case BoundNodeType.CallExpression:
                    WriteCallExpression((BoundCallExpression)expression);
                    break;
                case BoundNodeType.VariableExpression:
                    WriteVariableExpression((BoundVariableExpression)expression);
                    break;
                case BoundNodeType.LiteralExpression:
                    WriteLiteralExpression((BoundLiteralExpression)expression);
                    break;
                case BoundNodeType.AssignmentExpression:
                    WriteAssignmentExpression((BoundAssignmentExpression)expression);
                    break;
                case BoundNodeType.BinaryExpression:
                    WriteBinaryExpression((BoundBinaryExpression)expression);
                    break;
                case BoundNodeType.UnaryExpression:
                    WriteUnaryExpression((BoundUnaryExpression)expression);
                    break;
                default:
                    writer.Close(); // TODO delete file
                    throw new Exception($"Undefined expression {expression.boundType}");
            }
        }

        protected abstract void WriteUnaryExpression(BoundUnaryExpression expression);

        protected abstract void WriteBinaryExpression(BoundBinaryExpression expression);

        protected abstract void WriteAssignmentExpression(BoundAssignmentExpression expression);

        protected abstract void WriteLiteralExpression(BoundLiteralExpression expression);

        protected abstract void WriteVariableExpression(BoundVariableExpression expression);

        protected abstract void WriteCallExpression(BoundCallExpression expression);

        protected abstract void WriteConversionExpression(BoundConversionExpression expression);
    }
}