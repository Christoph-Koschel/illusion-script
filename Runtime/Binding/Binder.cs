using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticGroup diagnostics;
        private Scope scope;

        public Binder(Scope parent)
        {
            diagnostics = new DiagnosticGroup();
            scope = new Scope(parent);
        }

        public DiagnosticGroup Diagnostics => diagnostics;

        public BoundExpression BindExpression(Expression syntax)
        {
            switch (syntax.type)
            {
                case SyntaxType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpression)syntax);
                case SyntaxType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)syntax);
                case SyntaxType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpression)syntax);
                case SyntaxType.ParenExpression:
                    return BindParenExpression(((ParenExpression)syntax));
                case SyntaxType.NameExpression:
                    return BindNameExpression((NameExpression)syntax);
                case SyntaxType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpression)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.type}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpression syntax)
        {
            object value = syntax.value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression syntax)
        {
            BoundExpression right = BindExpression(syntax.right);
            BoundUnaryOperator unaryOperator = BoundUnaryOperator.Bind(syntax.operatorToken.type, right.type);
            if (unaryOperator == null)
            {
                diagnostics.ReportUndefinedUnaryOperator(syntax.operatorToken.span, syntax.operatorToken.text,
                    right.type);
                return right;
            }

            return new BoundUnaryExpression(unaryOperator, right);
        }


        private BoundExpression BindBinaryExpression(BinaryExpression syntax)
        {
            BoundExpression left = BindExpression(syntax.left);
            BoundExpression right = BindExpression(syntax.right);
            BoundBinaryOperator binaryOperator =
                BoundBinaryOperator.Bind(syntax.operatorToken.type, left.type, right.type);

            if (binaryOperator == null)
            {
                diagnostics.ReportUndefinedBinaryOperator(syntax.operatorToken.span, syntax.operatorToken.text,
                    left.type, right.type);
                return left;
            }

            return new BoundBinaryExpression(left, binaryOperator, right);
        }

        private BoundExpression BindParenExpression(ParenExpression syntax)
        {
            return BindExpression(syntax.expression);
        }

        private BoundExpression BindNameExpression(NameExpression syntax)
        {
            string name = syntax.identifier.text;

            if (!scope.TryLookup(name, out VariableSymbol variable))
            {
                diagnostics.ReportUndefinedIdentifier(syntax.identifier.span, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression syntax)
        {
            string name = syntax.identifier.text;
            BoundExpression boundExpression = BindExpression(syntax.expression);
            VariableSymbol variable = new VariableSymbol(name, boundExpression.type);

            if (!scope.TryDeclare(variable))
            {
                diagnostics.ReportVariableAlreadyDeclared(syntax.identifier.span, name);
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        public static GlobalScope BindGlobalScope(GlobalScope previous, CompilationUnit syntax)
        {
            Scope parentScope = CreateParentScopes(previous);
            Binder binder = new Binder(parentScope);
            BoundExpression expression = binder.BindExpression(syntax.expression);
            ImmutableArray<VariableSymbol> variables = binder.scope.GetDeclaredVariables();
            ImmutableArray<Diagnostic> diagnostics = binder.diagnostics.ToImmutableArray();

            return new GlobalScope(previous, diagnostics, variables, expression);
        }

        private static Scope CreateParentScopes(GlobalScope previous)
        {
            Stack<GlobalScope> stack = new Stack<GlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.previous;
            }

            Scope parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                Scope scope = new Scope(parent);
                foreach (VariableSymbol variable in previous.variables)
                {
                    scope.TryDeclare(variable);
                }

                parent = scope;
            }

            return parent;
        }
    }
}