using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;
using IllusionScript.Runtime.Parsing.Nodes.Statements;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class Binder
    {
        public readonly DiagnosticGroup diagnostics;
        private Scope scope;

        public Binder(Scope parent)
        {
            diagnostics = new DiagnosticGroup();
            scope = new Scope(parent);
        }

        #region Statements

        private BoundStatement BindStatement(Statement syntax)
        {
            switch (syntax.type)
            {
                case SyntaxType.BlockStatement:
                    return BindBlockStatement((BlockStatement)syntax);
                case SyntaxType.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatement)syntax);
                case SyntaxType.VariableDeclarationStatement:
                    return BindVariableDeclarationStatement((VariableDeclarationStatement)syntax);
                case SyntaxType.IfStatement:
                    return BindIfStatement((IfStatement)syntax);
                case SyntaxType.WhileStatement:
                    return BindWhileStatement((WhileStatement)syntax);
                case SyntaxType.ForStatement:
                    return BindForStatement((ForStatement)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.type}");
            }
        }

        private BoundStatement BindForStatement(ForStatement syntax)
        {
            BoundExpression startExpression = BindExpression(syntax.startExpression, TypeSymbol.Int);
            BoundExpression endExpression = BindExpression(syntax.endExpression, TypeSymbol.Int);

            scope = new Scope(scope);

            VariableSymbol variable = BindVariable(syntax.identifier, true, TypeSymbol.Int);
            BoundStatement body = BindStatement(syntax.body);

            scope = scope.parent;
            return new BoundForStatement(variable, startExpression, endExpression, body);
        }

        private BoundStatement BindWhileStatement(WhileStatement syntax)
        {
            BoundExpression condition = BindExpression(syntax.condition, TypeSymbol.Bool);
            BoundStatement statement = BindStatement(syntax.body);

            return new BoundWhileStatement(condition, statement);
        }

        private BoundStatement BindIfStatement(IfStatement syntax)
        {
            BoundExpression condition = BindExpression(syntax.condition, TypeSymbol.Bool);
            BoundStatement statement = BindStatement(syntax.body);
            BoundStatement elseStatement = syntax.elseClause == null ? null : BindStatement(syntax.elseClause.body);

            return new BoundIfStatement(condition, statement, elseStatement);
        }

        private BoundStatement BindVariableDeclarationStatement(VariableDeclarationStatement syntax)
        {
            bool isReadOnly = syntax.keyword.type == SyntaxType.ConstKeyword;
            BoundExpression initializer = BindExpression(syntax.initializer);
            VariableSymbol variable = BindVariable(syntax.identifier, isReadOnly, initializer.type);

            return new BoundVariableDeclarationStatement(variable, initializer);
        }

        private BoundStatement BindBlockStatement(BlockStatement syntax)
        {
            ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();
            scope = new Scope(scope);
            foreach (Statement statement in syntax.statements)
            {
                BoundStatement boundStatement = BindStatement(statement);
                statements.Add(boundStatement);
            }

            scope = scope.parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundStatement BindExpressionStatement(ExpressionStatement syntax)
        {
            BoundExpression expression = BindExpression(syntax.expression, true);
            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindExpression(Expression syntax, TypeSymbol target)
        {
            BoundExpression result = BindExpression(syntax);
            if (
                target != TypeSymbol.Error &&
                result.type != TypeSymbol.Error &&
                result.type != target)
            {
                diagnostics.ReportCannotConvert(syntax.span, result.type, target);
            }

            return result;
        }

        #endregion

        #region Expression

        private BoundExpression BindExpression(Expression syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);
            if (!canBeVoid && result.type == TypeSymbol.Void)
            {
                diagnostics.ReportExpressionMustHaveValue(syntax.span);
                return new BoundErrorExpression();
            }

            return result;
        }


        private BoundExpression BindExpressionInternal(Expression syntax)
        {
            switch (syntax.type)
            {
                case SyntaxType.ParenExpression:
                    return BindParenExpression((ParenExpression)syntax);
                case SyntaxType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpression)syntax);
                case SyntaxType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)syntax);
                case SyntaxType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpression)syntax);
                case SyntaxType.NameExpression:
                    return BindNameExpression((NameExpression)syntax);
                case SyntaxType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpression)syntax);
                case SyntaxType.CallExpression:
                    return BindCallExpression((CallExpression)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.type}");
            }
        }

        private BoundExpression BindCallExpression(CallExpression syntax)
        {
            if (syntax.arguments.Length == 1 && LookupType(syntax.identifier.text) is TypeSymbol type)
            {
                return BindConversion(type, syntax.arguments[0]);
            }

            ImmutableArray<BoundExpression>.Builder arguments = ImmutableArray.CreateBuilder<BoundExpression>();
            foreach (Expression argument in syntax.arguments)
            {
                BoundExpression boundArgument = BindExpression(argument);
                arguments.Add(boundArgument);
            }

            if (!scope.TryLookupFunction(syntax.identifier.text, out FunctionSymbol function))
            {
                diagnostics.ReportUndefinedFunction(syntax.identifier.span, syntax.identifier.text);
                return new BoundErrorExpression();
            }

            if (syntax.arguments.Length != function.parameters.Length)
            {
                diagnostics.ReportWrongArgumentCount(syntax.span, syntax.identifier.text, function.parameters.Length,
                    syntax.arguments.Length);
                return new BoundErrorExpression();
            }

            for (int i = 0; i < syntax.arguments.Length; i++)
            {
                BoundExpression argument = arguments[i];
                ParameterSymbol parameter = function.parameters[i];

                if (argument.type != parameter.type)
                {
                    diagnostics.WrongArgumentType(syntax.span, parameter.name, parameter.type, argument.type);
                    return new BoundErrorExpression();
                }
            }

            return new BoundCallExpression(function, arguments.ToImmutable());
        }

        private BoundExpression BindLiteralExpression(LiteralExpression syntax)
        {
            object value = syntax.value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression syntax)
        {
            BoundExpression right = BindExpression(syntax.right);

            if (right.type == TypeSymbol.Error)
            {
                return new BoundErrorExpression();
            }

            BoundUnaryOperator unaryOperator = BoundUnaryOperator.Bind(syntax.operatorToken.type, right.type);
            if (unaryOperator == null)
            {
                diagnostics.ReportUndefinedUnaryOperator(syntax.operatorToken.span, syntax.operatorToken.text,
                    right.type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(unaryOperator, right);
        }


        private BoundExpression BindBinaryExpression(BinaryExpression syntax)
        {
            BoundExpression left = BindExpression(syntax.left);
            BoundExpression right = BindExpression(syntax.right);

            if (left.type == TypeSymbol.Error || right.type == TypeSymbol.Error)
            {
                return new BoundErrorExpression();
            }

            BoundBinaryOperator binaryOperator =
                BoundBinaryOperator.Bind(syntax.operatorToken.type, left.type, right.type);

            if (binaryOperator == null)
            {
                diagnostics.ReportUndefinedBinaryOperator(syntax.operatorToken.span, syntax.operatorToken.text,
                    left.type, right.type);
                return new BoundErrorExpression();
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
            if (string.IsNullOrEmpty(name))
            {
                return new BoundErrorExpression();
            }

            if (!scope.TryLookupVariable(name, out VariableSymbol variable))
            {
                diagnostics.ReportUndefinedIdentifier(syntax.identifier.span, name);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression syntax)
        {
            string name = syntax.identifier.text;
            BoundExpression boundExpression = BindExpression(syntax.expression);

            if (!scope.TryLookupVariable(name, out VariableSymbol variable))
            {
                diagnostics.ReportUndefinedIdentifier(syntax.identifier.span, name);
                return boundExpression;
            }

            if (variable.isReadOnly)
            {
                diagnostics.ReportCannotAssign(syntax.identifier.span, name);
            }

            if (boundExpression.type != variable.type)
            {
                diagnostics.ReportCannotConvert(syntax.expression.span, boundExpression.type, variable.type);
                return boundExpression;
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        #endregion

        public static GlobalScope BindGlobalScope(GlobalScope previous, CompilationUnit syntax)
        {
            Scope parentScope = CreateParentScopes(previous);
            Binder binder = new Binder(parentScope);
            BoundStatement expression = binder.BindStatement(syntax.statement);
            ImmutableArray<VariableSymbol> variables = binder.scope.GetDeclaredVariables();
            ImmutableArray<Diagnostic> diagnostics = binder.diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.diagnostics);
            }

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

            Scope parent = CreateRootScope();


            while (stack.Count > 0)
            {
                previous = stack.Pop();
                Scope scope = new Scope(parent);
                foreach (VariableSymbol variable in previous.variables)
                {
                    scope.TryDeclareVariable(variable);
                }

                parent = scope;
            }

            return parent;
        }

        private static Scope CreateRootScope()
        {
            var result = new Scope(null);
            foreach (FunctionSymbol symbol in BuiltInFunctions.GetAll())
            {
                result.TryDeclareFunction(symbol);
            }

            return result;
        }


        private VariableSymbol BindVariable(Token identifier, bool isReadOnly, TypeSymbol type)
        {
            string name = identifier.text ?? "?";
            bool declare = !identifier.isMissing;
            VariableSymbol variable = new VariableSymbol(name, isReadOnly, type);

            if (declare && !scope.TryDeclareVariable(variable))
            {
                diagnostics.ReportVariableAlreadyDeclared(identifier.span, name);
            }

            return variable;
        }

        private BoundExpression BindConversion(TypeSymbol type, Expression syntax)
        {
            var expression = BindExpression(syntax);
            var conversion = Conversion.Classify(expression.type, type);
            if (!conversion.exists)
            {
                diagnostics.ReportCannotConvert(syntax.span, expression.type, type);
                return new BoundErrorExpression();
            }

            return new BoundConversionExpression(type, expression);
        }

        private TypeSymbol LookupType(string name)
        {
            return name switch
            {
                "Bool" => TypeSymbol.Bool,
                "Int" => TypeSymbol.Int,
                "String" => TypeSymbol.String,
                _ => null
            };
        }
    }
}