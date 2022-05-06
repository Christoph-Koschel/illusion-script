using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Interpreting
{
    internal sealed class Interpreter
    {
        private readonly BoundProgram program;
        private readonly Dictionary<VariableSymbol, object> globals;
        private readonly Dictionary<FunctionSymbol, BoundBlockStatement> functions;

        private readonly Stack<Dictionary<VariableSymbol, object>> locals;
        private object lastValue;
        private Random random;

        public Interpreter(BoundProgram program, Dictionary<VariableSymbol, object> globals)
        {
            functions = new Dictionary<FunctionSymbol, BoundBlockStatement>();
            this.program = program;
            this.globals = globals;
            locals = new Stack<Dictionary<VariableSymbol, object>>();
            locals.Push(new Dictionary<VariableSymbol, object>());

            var current = program;
            while (current != null)
            {
                foreach (KeyValuePair<FunctionSymbol, BoundBlockStatement> functionBody in current.functionBodies)
                {
                    functions.Add(functionBody.Key, functionBody.Value);
                }

                current = current.previous;
            }
        }

        public object Interpret()
        {
            return InterpretStatement(program.statement);
        }

        private object InterpretStatement(BoundBlockStatement body)
        {
            Dictionary<BoundLabel, int> labelToIndex = new Dictionary<BoundLabel, int>();

            for (int i = 0; i < body.statements.Length; i++)
            {
                if (body.statements[i] is BoundLabelStatement l)
                {
                    labelToIndex.Add(l.BoundLabel, i + 1);
                }
            }

            int index = 0;
            while (index < body.statements.Length)
            {
                BoundStatement statement = body.statements[index];

                switch (statement.boundType)
                {
                    case BoundNodeType.ExpressionStatement:
                        InterpretExpressionStatement((BoundExpressionStatement)statement);
                        index++;
                        break;
                    case BoundNodeType.VariableDeclarationStatement:
                        InterpretVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                        index++;
                        break;
                    case BoundNodeType.ConditionalGotoStatement:
                    {
                        BoundConditionalGotoStatement cgs = (BoundConditionalGotoStatement)statement;
                        bool condition = (bool)InterpretExpression(cgs.condition);
                        if (condition == cgs.jmpIfTrue)
                        {
                            index = labelToIndex[cgs.boundLabel];
                        }
                        else
                        {
                            index++;
                        }

                        break;
                    }
                    case BoundNodeType.GotoStatement:
                    {
                        BoundGotoStatement gs = (BoundGotoStatement)statement;

                        index = labelToIndex[gs.BoundLabel];
                        break;
                    }
                    case BoundNodeType.LabelStatement:
                        index++;
                        break;
                    case BoundNodeType.ReturnStatement:
                        BoundReturnStatement rs = (BoundReturnStatement)statement;
                        lastValue = rs.expression == null ? null : InterpretExpression(rs.expression);
                        return lastValue;
                    default:
                        throw new Exception($"Unexpected node {statement.boundType}");
                }
            }

            return lastValue;
        }

        private void InterpretVariableDeclarationStatement(BoundVariableDeclarationStatement statement)
        {
            object value = InterpretExpression(statement.initializer);
            lastValue = value;

            Assign(statement.variable, value);
        }

        private void InterpretExpressionStatement(BoundExpressionStatement statement)
        {
            lastValue = InterpretExpression(statement.expression);
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
                BoundCallExpression c => InterpretCallExpression(c),
                BoundConversionExpression co => InterpretConversionExpression(co),
                _ => throw new Exception($"Unexpected node {node.type}")
            };
        }

        private object InterpretConversionExpression(BoundConversionExpression co)
        {
            object value = InterpretExpression(co.expression);

            if (co.type == TypeSymbol.Object)
            {
                return value;
            }
            else if (co.type == TypeSymbol.Bool)
            {
                return Convert.ToBoolean(value);
            }
            else if (co.type == TypeSymbol.Int)
            {
                return Convert.ToInt32(value);
            }
            else if (co.type == TypeSymbol.String)
            {
                return Convert.ToString(value);
            }
            else
            {
                throw new Exception($"Unexpected type {co.type}");
            }
        }

        private object InterpretCallExpression(BoundCallExpression c)
        {
            if (c.function == BuiltInFunctions.Scan)
            {
                return Console.ReadLine();
            }
            else if (c.function == BuiltInFunctions.Print)
            {
                string value = (string)InterpretExpression(c.arguments[0]);
                Console.WriteLine(value);
                return null;
            }
            else if (c.function == BuiltInFunctions.Rand)
            {
                int max = (int)InterpretExpression(c.arguments[0]);
                if (random == null)
                {
                    random = new Random();
                }

                return random.Next(max);
            }
            else
            {
                Dictionary<VariableSymbol, object> frame = new Dictionary<VariableSymbol, object>();
                for (int i = 0; i < c.arguments.Length; i++)
                {
                    ParameterSymbol parameter = c.function.parameters[i];
                    object value = InterpretExpression(c.arguments[i]);
                    frame.Add(parameter, value);
                }

                locals.Push(frame);
                BoundBlockStatement statement = functions[c.function];
                object result = InterpretStatement(statement);
                locals.Pop();
                return result;
            }
        }

        private object InterpretAssignmentExpression(BoundAssignmentExpression a)
        {
            object value = InterpretExpression(a.expression);
            Assign(a.variableSymbol, value);

            return value;
        }

        private object InterpretVariableExpression(BoundVariableExpression v)
        {
            if (v.variableSymbol.symbolType is SymbolType.GlobalVariable)
            {
                return globals[v.variableSymbol];
            }

            Dictionary<VariableSymbol, object> frame = locals.Peek();
            return frame[v.variableSymbol];
        }

        private object InterpretBinaryExpression(BoundBinaryExpression b)
        {
            object left = InterpretExpression(b.left);
            object right = InterpretExpression(b.right);

            switch (b.binaryOperator.operatorType)
            {
                case BoundBinaryOperatorType.Addition:
                    if (b.type == TypeSymbol.Int)
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
                    if (b.type == TypeSymbol.Int)
                    {
                        return (int)left & (int)right;
                    }
                    else
                    {
                        return (bool)left & (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseOr:
                    if (b.type == TypeSymbol.Int)
                    {
                        return (int)left | (int)right;
                    }
                    else
                    {
                        return (bool)left | (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseXor:
                    if (b.type == TypeSymbol.Int)
                    {
                        return (int)left ^ (int)right;
                    }
                    else
                    {
                        return (bool)left ^ (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseShiftLeft:
                    return (int)left << (int)right;
                case BoundBinaryOperatorType.BitwiseShiftRight:
                    return (int)left >> (int)right;
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

        private void Assign(VariableSymbol symbol, object value)
        {
            if (symbol.symbolType is SymbolType.GlobalVariable)
            {
                globals[symbol] = value;
            }
            else
            {
                Dictionary<VariableSymbol, object> frame = locals.Peek();
                frame[symbol] = value;
            }
        }
    }
}