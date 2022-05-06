using System;
using System.Collections;
using System.Collections.Generic;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Diagnostics
{
    internal class DiagnosticGroup : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> diagnostics;

        public DiagnosticGroup()
        {
            diagnostics = new List<Diagnostic>();
        }

        private void Report(TextLocation location, string message)
        {
            Diagnostic diagnostic = new Diagnostic(location, message);
            diagnostics.Add(diagnostic);
        }

        public void AddRange(DiagnosticGroup diagnosticGroup)
        {
            diagnostics.AddRange(diagnosticGroup.diagnostics);
        }

        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ReportInvalidNumber(TextLocation location, string text, Type type)
        {
            string message = $"ERROR: The number {text} isn't valid {type}";
            Report(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char current)
        {
            string message = $"ERROR: Bad character input: '{current}'";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            string message = "ERROR: Unterminated string literal";
            Report(location, message);
        }

        public void ReportUnexpectedToken(TextLocation location, SyntaxType currentType, SyntaxType type)
        {
            string message = $"ERROR: Unexpected token <{currentType}>, expected <{type}>";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string text, TypeSymbol right)
        {
            string message = $"ERROR: Unary operator '{text}' is not defined for type {right}";
            Report(location, message);
        }

        public void ReportUndefinedBinaryOperator(TextLocation location, string text, TypeSymbol left, TypeSymbol right)
        {
            string message = $"ERROR: Binary operator '{text}' is not defined for type {left} and {right}";
            Report(location, message);
        }

        public void ReportUndefinedIdentifier(TextLocation location, string name)
        {
            string message = $"ERROR: Variable '{name}' doesnt exist";
            Report(location, message);
        }

        public void ReportCannotConvert(TextLocation location, TypeSymbol type1, TypeSymbol type2)
        {
            string message = $"ERROR: Cannot convert type '{type1}' to '{type2}'";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"ERROR: Symbol '{name}' is already declared";
            Report(location, message);
        }

        public void ReportParameterAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"ERROR: A Parameter with the '{name}' is already defined";
            Report(location, message);
        }

        public void ReportCannotAssign(TextLocation location, string name)
        {
            string message = $"ERROR: Variable '{name}' is read-ony and cannot be assigned to";
            Report(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string name)
        {
            string message = $"ERROR: Function '{name}' doesnt exists";
            Report(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string text)
        {
            string message = $"ERROR: Type '{text}' doesnt exists";
            Report(location, message);
        }

        public void ReportWrongArgumentCount(TextLocation location, string name, int parametersLength,
            int argumentsLength)
        {
            string message =
                $"ERROR: Function '{name}' requires {parametersLength} arguments but was given {argumentsLength}";
            Report(location, message);
        }

        public void ReportExpressionMustHaveValue(TextLocation location)
        {
            string message = $"ERROR: Expression must have a value";
            Report(location, message);
        }

        public void ReportCannotConvertConvertImplicitly(TextLocation location, TypeSymbol type1, TypeSymbol type2)
        {
            string message =
                $"ERROR: Cannot convert type '{type1}' to '{type2}' (Are you missing a cast?)";
            Report(location, message);
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string text)
        {
            string message = $"The keyword '{text}' is only valid inside loops";
            Report(location, text);
        }

        public void ReportInvalidReturn(TextLocation location)
        {
            string message = "ERROR: The 'return' keyword can only be used inside of functions";
            Report(location, message);
        }

        public void ReportInvalidReturnExpression(TextLocation location, string name)
        {
            string message =
                $"ERROR: Since the function '{name}' does not return a value the 'return' keyword cannot be followed by an expression";
            Report(location, message);
        }

        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            string message = $"ERROR: An expression of type '{returnType}' expected";
            Report(location, message);
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            string message = $"ERROR: Not all code paths return a value.";
            Report(location, message);
        }

        public void ReportInvalidExpressionStatement(TextLocation location)
        {
            string message = "ERROR: Only assignment and call expressions can be used as a statement";
            Report(location, message);
        }

        public void ReportCannotMixMainAndGlobalStatements(TextLocation location)
        {
            string message = "ERROR: Cannot declare a main function when global statements are used";
            Report(location, message);
        }

        public void ReportMainMustHaveCorrectSignature(TextLocation location)
        {
            string message = "ERROR: Main must not take arguments and not return any anything";
            Report(location, message);
        }

        public void ReportOnlyOneFileCanHavaGlobalStatements(TextLocation location)
        {
            string message = "ERROR: At most one file can have global statements";
            Report(location, message);
        }
    }
}