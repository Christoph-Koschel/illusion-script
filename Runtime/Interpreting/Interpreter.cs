using System;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Node;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Symbols;

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
                case BoundUnaryOperatorType.OnesComplement:
                    return ~(int)right;
                case BoundUnaryOperatorType.LogicalNegation:
                    return !(bool)right;
                default:
                    throw new Exception($"Undefined unary operator: {expression.unaryOperator.operatorType}");
            }
        }

        private object InterpretBinaryExpression(BoundBinary expression)
        {
            object left = InterpretExpression(expression.left);
            object right = InterpretExpression(expression.right);

            switch (expression.binaryOperator.operatorType)
            {
                case BoundBinaryOperatorType.Addition:
                    if (expression.type == TypeSymbol.Int)
                    {
                        return (int)left + (int)right;
                    }
                    else
                    {
                        return (string)left + (string)right;
                    }
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
                    int x = (int)left;
                    int pow = (int)right;
                    int ret = 1;
                    while (pow != 0)
                    {
                        if ((pow & 1) == 1)
                            ret *= x;
                        x *= x;
                        pow >>= 1;
                    }

                    return ret;
                }
                case BoundBinaryOperatorType.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorType.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorType.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorType.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorType.BitwiseAnd:
                    if (expression.type == TypeSymbol.Int)
                    {
                        return (int)left & (int)right;
                    }
                    else
                    {
                        return (bool)left & (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseOr:
                    if (expression.type == TypeSymbol.Int)
                    {
                        return (int)left | (int)right;
                    }
                    else
                    {
                        return (bool)left | (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseXor:
                    if (expression.type == TypeSymbol.Int)
                    {
                        return (int)left ^ (int)right;
                    }
                    else
                    {
                        return (bool)left ^ (bool)right;
                    }
                case BoundBinaryOperatorType.Less:
                    return (int)left < (int)right;
                case BoundBinaryOperatorType.LessEquals:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorType.Greater:
                    return (int)left > (int)right;
                case BoundBinaryOperatorType.GreaterEquals:
                    return (int)left >= (int)right;
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