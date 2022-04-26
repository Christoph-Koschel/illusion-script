using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
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

        private void Report(TextSpan span, string message)
        {
            Diagnostic diagnostic = new Diagnostic(span, message);
            diagnostics.Add(diagnostic);
        }

        public void AddRange(DiagnosticGroup diagnosticGroup)
        {
            diagnostics.AddRange(diagnosticGroup.diagnostics);
        }

        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            string message = $"ERROR: The number {text} isn't valid {type}";
            Report(span, message);
        }

        public void ReportBadCharacter(int position, char current)
        {
            string message = $"ERROR: Bad character input: '{current}'";
            TextSpan span = new TextSpan(position, 1);
            Report(span, message);
        }

        public void ReportUnterminatedString(TextSpan span)
        {
            string message = "ERROR: Unterminated string literal";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxType currentType, SyntaxType type)
        {
            string message = $"ERROR: Unexpected token <{currentType}>, expected <{type}>";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string text, TypeSymbol right)
        {
            string message = $"ERROR: Unary operator '{text}' is not defined for type {right}";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string text, TypeSymbol left, TypeSymbol right)
        {
            string message = $"ERROR: Binary operator '{text}' is not defined for type {left} and {right}";
            Report(span, message);
        }

        public void ReportUndefinedIdentifier(TextSpan span, string name)
        {
            string message = $"ERROR: Variable '{name}' doesnt exist";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, TypeSymbol type1, TypeSymbol type2)
        {
            string message = $"ERROR: Cannot convert type '{type1}' to '{type2}'";
            Report(span, message);
        }

        public void ReportSymbolAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"ERROR: Symbol '{name}' is already declared";
            Report(span, message);
        }
        
        public void ReportParameterAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"ERROR: A Parameter with the '{name}' is already defined";
            Report(span, message);
        }

        public void ReportCannotAssign(TextSpan span, string name)
        {
            string message = $"ERROR: Variable '{name}' is read-ony and cannot be assigned to";
            Report(span, message);
        }

        public void ReportUndefinedFunction(TextSpan span, string name)
        {
            string message = $"ERROR: Function '{name}' doesnt exists";
            Report(span, message);
        }

        public void ReportUndefinedType(TextSpan span, string text)
        {
            string message = $"ERROR: Type '{text}' doesnt exists";
            Report(span, message);
        }

        public void ReportWrongArgumentCount(TextSpan span, string name, int parametersLength, int argumentsLength)
        {
            string message = $"ERROR: Function '{name}' requires {parametersLength} arguments but was given {argumentsLength}";
            Report(span, message);
        }

        public void WrongArgumentType(TextSpan span, string name, TypeSymbol parameterType, TypeSymbol argumentType)
        {
            string message =
                $"ERROR: Parameter '{name}' requires a value of type '{parameterType}' but was given a value of type '{argumentType}'";
            Report(span, message);
        }

        public void ReportExpressionMustHaveValue(TextSpan span)
        {
            string message = $"ERROR: Expression must have a value";
            Report(span, message);
        }

        public void ReportCannotConvertConvertImplicitly(TextSpan span, TypeSymbol type1, TypeSymbol type2)
        {
            string message =
                $"ERROR: Cannot convert type '{type1}' to '{type2}' (Are you missing a cast?)";
            Report(span, message);
        }

        public void XXX_ReportFunctionsAreUnsupported(TextSpan span)
        {
            string message = "ERROR: Functions with return values are unsupported";
            Report(span, message);
        }
    }
}