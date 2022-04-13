using System.Collections;
using System.Collections.Generic;

namespace IllusionScript.Runtime.Diagnostics
{
    public sealed class DiagnosticGroup : IEnumerable<Diagnostic>
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
        
        public void ReportBadCharacter(int position, char current)
        {
            string message = $"ERROR: Bad character input: '{current}'";
            TextSpan span = new TextSpan(position, 1);
            Report(span, message);
        }
    }
}