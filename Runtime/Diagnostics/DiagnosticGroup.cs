using System;
using System.Collections;
using System.Collections.Generic;
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

        public void ReportUnexpectedToken(TextSpan span, SyntaxType currentType, SyntaxType type)
        {
            string message = $"ERROR: Unexpected token <{currentType}>, expected <{type}>";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string text, Type right)
        {
            string message = $"ERROR: Unary operator '{text}' is not defined for type {right}";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string text, Type left, Type right)
        {
            string message = $"ERROR: Binary operator '{text}' is not defined for type {left} and {right}";
            Report(span, message);
        }

        public void ReportUndefinedIdentifier(TextSpan span, string name)
        {
            string message = $"ERROR: Variable '{name}' doesnt exist";
            Report(span, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"ERROR: Variable '{name}' already declared";
            Report(span, message);
        }
    }
}