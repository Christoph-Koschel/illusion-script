using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Lowering;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;
using IllusionScript.Runtime.Parsing.Nodes.Members;
using IllusionScript.Runtime.Parsing.Nodes.Statements;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class Binder
    {
        public readonly FunctionSymbol function;
        public readonly DiagnosticGroup diagnostics;
        private readonly Stack<(BoundLabel breakLabel, BoundLabel continueLabel)> loopStack;
        private int labelCounter;

        private Scope scope;

        public Binder(Scope parent, FunctionSymbol function)
        {
            this.function = function;
            diagnostics = new DiagnosticGroup();
            scope = new Scope(parent);
            loopStack = new Stack<(BoundLabel breakLabel, BoundLabel continueLabel)>();
            labelCounter = 0;

            if (function != null)
            {
                foreach (ParameterSymbol parameter in function.parameters)
                {
                    scope.TryDeclareVariable(parameter);
                }
            }
        }

        #region Statements

        private BoundStatement BindErrorStatement()
        {
            return new BoundExpressionStatement(new BoundErrorExpression());
        }

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
                case SyntaxType.DoWhileStatement:
                    return BindDoWhileStatement((DoWhileStatement)syntax);
                case SyntaxType.ForStatement:
                    return BindForStatement((ForStatement)syntax);
                case SyntaxType.ContinueStatement:
                    return BindContinueStatement((ContinueStatement)syntax);
                case SyntaxType.BreakStatement:
                    return BindBreakStatement((BreakStatement)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.type}");
            }
        }

        private BoundStatement BindBreakStatement(BreakStatement syntax)
        {
            if (loopStack.Count == 0)
            {
                diagnostics.ReportInvalidBreakOrContinue(syntax.keyword.span, syntax.keyword.text);
                return BindErrorStatement();
            }

            BoundLabel breakLabel = loopStack.Peek().breakLabel;
            return new BoundGotoStatement(breakLabel);
        }

        private BoundStatement BindContinueStatement(ContinueStatement syntax)
        {
            if (loopStack.Count == 0)
            {
                diagnostics.ReportInvalidBreakOrContinue(syntax.keyword.span, syntax.keyword.text);
                return BindErrorStatement();
            }

            BoundLabel continueLabel = loopStack.Peek().continueLabel;
            return new BoundGotoStatement(continueLabel);
        }

        private BoundStatement BindForStatement(ForStatement syntax)
        {
            BoundExpression startExpression = BindExpression(syntax.startExpression, TypeSymbol.Int);
            BoundExpression endExpression = BindExpression(syntax.endExpression, TypeSymbol.Int);

            scope = new Scope(scope);

            VariableSymbol variable = BindVariable(syntax.identifier, true, TypeSymbol.Int);
            BoundStatement body = BindLoopBody(syntax.body, out BoundLabel breakLabel, out BoundLabel continueLabel);

            scope = scope.parent;
            return new BoundForStatement(variable, startExpression, endExpression, body, breakLabel, continueLabel);
        }

        private BoundStatement BindDoWhileStatement(DoWhileStatement syntax)
        {
            BoundStatement body = BindLoopBody(syntax.body, out BoundLabel breakLabel, out BoundLabel continueLabel);
            BoundExpression condition = BindExpression(syntax.condition, TypeSymbol.Bool);
            return new BoundDoWhileStatement(body, condition, breakLabel, continueLabel);
        }

        private BoundStatement BindWhileStatement(WhileStatement syntax)
        {
            BoundExpression condition = BindExpression(syntax.condition, TypeSymbol.Bool);
            BoundStatement body = BindLoopBody(syntax.body, out BoundLabel breakLabel, out BoundLabel continueLabel);

            return new BoundWhileStatement(condition, body, breakLabel, continueLabel);
        }

        private BoundStatement BindLoopBody(Statement body, out BoundLabel breakLabel, out BoundLabel continueLabel)
        {
            breakLabel = new BoundLabel("b" + labelCounter);
            continueLabel = new BoundLabel("c" + labelCounter);
            labelCounter++;

            loopStack.Push((breakLabel, continueLabel));
            BoundStatement boundBody = BindStatement(body);
            loopStack.Pop();
            return boundBody;
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
            TypeSymbol type = BindTypeClause(syntax.typeClause);
            BoundExpression initializer = BindExpression(syntax.initializer);
            TypeSymbol variableType = type ?? initializer.type;
            VariableSymbol variable = BindVariable(syntax.identifier, isReadOnly, variableType);
            BoundExpression convertedInitializer = BindConversion(syntax.initializer.span, initializer, variableType);

            return new BoundVariableDeclarationStatement(variable, convertedInitializer);
        }

        private TypeSymbol BindTypeClause(TypeClause syntax, bool enableVoid = false)
        {
            TypeSymbol type = LookupType(syntax.identifier.text, enableVoid);
            if (type == null && !string.IsNullOrEmpty(syntax.identifier.text))
            {
                diagnostics.ReportUndefinedType(syntax.identifier.span, syntax.identifier.text);
            }

            return type;
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
            return BindConversion(syntax, target);
        }

        #endregion

        #region Expression

        private BoundExpression BindExpression(Expression syntax, bool canBeVoid = false)
        {
            BoundExpression result = BindExpressionInternal(syntax);
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
                return BindConversion(syntax.arguments[0], type, true);
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
                    diagnostics.WrongArgumentType(syntax.arguments[i].span, parameter.name, parameter.type,
                        argument.type);
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

            BoundExpression convertedExpression =
                BindConversion(syntax.expression.span, boundExpression, variable.type);
            return new BoundAssignmentExpression(variable, convertedExpression);
        }

        #endregion

        private void BindFunctionDeclarationMember(FunctionDeclarationMember syntax)
        {
            ImmutableArray<ParameterSymbol>.Builder parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();
            HashSet<string> seenParameterName = new HashSet<string>();
            foreach (Parameter parameter in syntax.parameters)
            {
                string parameterName = parameter.identifier.text;
                TypeSymbol parameterType = BindTypeClause(parameter.typeClause);
                if (!seenParameterName.Add(parameterName))
                {
                    diagnostics.ReportParameterAlreadyDeclared(parameter.span, parameterName);
                }
                else
                {
                    ParameterSymbol p = new ParameterSymbol(parameterName, parameterType);
                    parameters.Add(p);
                }
            }

            TypeSymbol type = BindTypeClause(syntax.typeClause, true) ?? TypeSymbol.Void;
            if (type != TypeSymbol.Void)
            {
                diagnostics.XXX_ReportFunctionsAreUnsupported(syntax.span);
            }

            FunctionSymbol function =
                new FunctionSymbol(syntax.identifier.text, parameters.ToImmutable(), type, syntax);
            if (!scope.TryDeclareFunction(function))
            {
                diagnostics.ReportSymbolAlreadyDeclared(syntax.identifier.span, function.name);
            }
        }


        public static BoundProgram BindProgram(GlobalScope globalScope)
        {
            Scope parentScope = CreateParentScopes(globalScope);
            ImmutableDictionary<FunctionSymbol, BoundBlockStatement>.Builder functionBodies =
                ImmutableDictionary.CreateBuilder<FunctionSymbol, BoundBlockStatement>();
            DiagnosticGroup diagnostics = new DiagnosticGroup();

            GlobalScope scope = globalScope;
            while (scope != null)
            {
                foreach (FunctionSymbol function in scope.functions)
                {
                    Binder binder = new Binder(parentScope, function);
                    BoundStatement body = binder.BindStatement(function.declaration.body);
                    BoundBlockStatement loweredBody = Lowerer.Lower(body);
                    functionBodies.Add(function, loweredBody);

                    diagnostics.AddRange(binder.diagnostics);
                }

                scope = scope.previous;
            }

            var statement = Lowerer.Lower(new BoundBlockStatement(globalScope.statements));

            return new BoundProgram(globalScope, diagnostics, functionBodies.ToImmutable(), statement);
        }

        public static GlobalScope BindGlobalScope(GlobalScope previous, CompilationUnit syntax)
        {
            Scope parentScope = CreateParentScopes(previous);
            Binder binder = new Binder(parentScope, null);

            foreach (FunctionDeclarationMember function in syntax.members.OfType<FunctionDeclarationMember>())
            {
                binder.BindFunctionDeclarationMember(function);
            }

            ImmutableArray<BoundStatement>.Builder statementBuilder = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (StatementMember statementMember in syntax.members.OfType<StatementMember>())
            {
                BoundStatement s = binder.BindStatement(statementMember.statement);
                statementBuilder.Add(s);
            }

            ImmutableArray<BoundStatement> statements = statementBuilder.ToImmutable();

            ImmutableArray<FunctionSymbol> functions = binder.scope.GetDeclaredFunctions();
            ImmutableArray<VariableSymbol> variables = binder.scope.GetDeclaredVariables();
            ImmutableArray<Diagnostic> diagnostics = binder.diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.diagnostics);
            }

            return new GlobalScope(previous, diagnostics, variables, functions, statements);
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
                foreach (FunctionSymbol function in previous.functions)
                {
                    scope.TryDeclareFunction(function);
                }


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
            Scope result = new Scope(null);
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
            VariableSymbol variable = function == null
                ? new GlobalVariableSymbol(name, isReadOnly, type)
                : new LocalVariableSymbol(name, isReadOnly, type);

            if (declare && !scope.TryDeclareVariable(variable))
            {
                diagnostics.ReportSymbolAlreadyDeclared(identifier.span, name);
            }

            return variable;
        }

        private BoundExpression BindConversion(Expression syntax, TypeSymbol type, bool allowExplicit = false)
        {
            BoundExpression expression = BindExpression(syntax);
            return BindConversion(syntax.span, expression, type, allowExplicit);
        }

        private BoundExpression BindConversion(TextSpan span, BoundExpression expression, TypeSymbol type,
            bool allowExplicit = false)
        {
            Conversion conversion = Conversion.Classify(expression.type, type);

            if (!conversion.exists)
            {
                if (expression.type != TypeSymbol.Error && type != TypeSymbol.Error)
                {
                    diagnostics.ReportCannotConvert(span, expression.type, type);
                }

                return new BoundErrorExpression();
            }

            if (!allowExplicit && conversion.isExplicit)
            {
                diagnostics.ReportCannotConvertConvertImplicitly(span, expression.type, type);
            }

            if (conversion.isIdentity)
            {
                return expression;
            }

            return new BoundConversionExpression(type, expression);
        }

        private TypeSymbol LookupType(string name, bool enableVoid = false)
        {
            if (enableVoid && name == "Void")
            {
                return TypeSymbol.Void;
            }

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