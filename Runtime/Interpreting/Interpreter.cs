﻿using System;
using System.Collections.Generic;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Interpreting.Memory;

namespace IllusionScript.Runtime.Interpreting
{
    internal sealed class Interpreter
    {
        private readonly BoundExpression root;
        private readonly Dictionary<VariableSymbol, object> variables;

        public Interpreter(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            this.root = root;
            this.variables = variables;
        }

        public object Interpret()
        {
            return InterpretExpression(root);
        }

        private object InterpretExpression(BoundExpression node)
        {
            return node switch
            {
                BoundLiteralExpression n => InterpretLiteralExpression(n),
                BoundUnaryExpression u => InterpretUnaryExpression(u),
                BoundBinaryExpression b => InterpretBinaryExpression(b),
                BoundVariableExpression v => InterpretVariableExpression(v),
                BoundAssignmentExpression a => InterpretAssignmentExpression(a),
                _ => throw new Exception($"Unexpected node {node.type}")
            };
        }

        private object InterpretAssignmentExpression(BoundAssignmentExpression a)
        {
            object value = InterpretExpression(a.expression);
            variables[a.variableSymbol] = value;
            return value;
        }

        private object InterpretVariableExpression(BoundVariableExpression v)
        {
            return variables[v.variableSymbol];
        }

        private object InterpretBinaryExpression(BoundBinaryExpression b)
        {
            object left = InterpretExpression(b.left);
            object right = InterpretExpression(b.right);

            switch (b.binaryOperator.operatorType)
            {
                case BoundBinaryOperatorType.Addition:
                    if (b.type == typeof(int))
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
                    if (b.type == typeof(int))
                    {
                        return (int)left & (int)right;
                    }
                    else
                    {
                        return (bool)left & (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseOr:
                    if (b.type == typeof(int))
                    {
                        return (int)left | (int)right;
                    }
                    else
                    {
                        return (bool)left | (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseXor:
                    if (b.type == typeof(int))
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
                    throw new Exception($"Undefined binary operator: {b.binaryOperator.operatorType}");
            }
        }

        private object InterpretUnaryExpression(BoundUnaryExpression u)
        {
            object right = InterpretExpression(u.right);
            return u.unaryOperator.operatorType switch
            {
                BoundUnaryOperatorType.Identity => right,
                BoundUnaryOperatorType.Negation => -(int)right,
                BoundUnaryOperatorType.OnesComplement => ~(int)right,
                BoundUnaryOperatorType.LogicalNegation => !(bool)right,
                _ => throw new Exception($"Undefined unary operator: {u.unaryOperator.operatorType}")
            };
        }

        private static object InterpretLiteralExpression(BoundLiteralExpression n)
        {
            return n.value;
        }
    }
}