using System;
using IllusionScript.Runtime.Binding.Node;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class Binder
    {
        public readonly DiagnosticGroup diagnostics;

        public Binder()
        {
            diagnostics = new DiagnosticGroup();
        }

        public BoundExpression Bind(Expression expression)
        {
            switch (expression.type)
            {
                case SyntaxType.NumberExpression:
                    return BindNumberExpression((NumberExpression)expression);
                case SyntaxType.BinaryExpression:
                    return BindBinaryExpression((BinaryOperationExpression)expression);
                case SyntaxType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)expression);
                case SyntaxType.ParenExpression:
                    return Bind(((ParenExpression)expression).expression);
                default:
                    throw new Exception("Undefined expression for binder");
            }
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expression)
        {
            BoundExpression right = Bind(expression.factor);
            BoundUnaryOperator unaryOperator = BoundUnaryOperator.Bind(expression.operatorToken.type, right.type);
            return new BoundUnary(unaryOperator, right);
        }

        private BoundExpression BindBinaryExpression(BinaryOperationExpression expression)
        {
            BoundExpression left = Bind(expression.left);
            BoundExpression right = Bind(expression.right);

            BoundBinaryOperator binaryOperator =
                BoundBinaryOperator.Bind(expression.operatorToken.type, left.type, right.type);
            return new BoundBinary(left, binaryOperator, right);
        }

        private BoundExpression BindNumberExpression(NumberExpression expression)
        {
            return new BoundLiteral(expression.number.value);
        }
    }
}