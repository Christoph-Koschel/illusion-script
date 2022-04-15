using System;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Node;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Diagnostics;

namespace IllusionScript.Runtime.Interpreting
{
    internal sealed class Interpreter
    {
        private readonly BoundExpression expression;
        public readonly DiagnosticGroup diagnostic;
        
        internal Interpreter(BoundExpression expression)
        {
            this.expression = expression;
            diagnostic = new DiagnosticGroup();
        }

        public object Interpret()
        {
            return InterpretExpression(expression);
        }

        private object InterpretExpression(BoundExpression expression)
        {
            switch (expression.boundType)
            {
                case BoundNodeType.LiteralExpression:
                    return InterpretLiteralExpression((BoundLiteral)expression);
                case BoundNodeType.BinaryExpression:
                    return InterpretBinaryExpression((BoundBinary)expression);
                case BoundNodeType.UnaryExpression:
                    return InterpretUnaryExpression((BoundUnary)expression);
                default:
                    throw new Exception($"Undefined expression type {expression.boundType}");
            }
        }

        private object InterpretUnaryExpression(BoundUnary expression)
        {
            object right = InterpretExpression(expression.right);

            switch (expression.unaryOperator.operatorType)
            {
                case BoundUnaryOperatorType.Identity:
                    return right;
                case BoundUnaryOperatorType.Negation:
                    return -(int)right;
                case BoundUnaryOperatorType.LogicalNegation:
                    return !(bool)right;
                default:
                    throw new Exception($"Undefined binary operator: {expression.unaryOperator.operatorType}");
            }
        }

        private object InterpretBinaryExpression(BoundBinary expression)
        {
            object left = InterpretExpression(expression.left);
            object right = InterpretExpression(expression.right);

            switch (expression.binaryOperator.operatorType)
            {
                case BoundBinaryOperatorType.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorType.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorType.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorType.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorType.Modulo:
                    return (int)left % (int)right;
                case BoundBinaryOperatorType.Pow:
                {
                    int index = (int)right;
                    int result = 0;
                    while (index != 0)
                    {
                        result *= (int)left;
                        index--;
                    }

                    return result;
                }
                default:
                    throw new Exception($"Undefined binary operator: {expression.binaryOperator.operatorType}");
            }
        }

        private object InterpretLiteralExpression(BoundLiteral expression)
        {
            return expression.value;
        }
    }
}