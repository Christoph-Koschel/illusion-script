using IllusionScript.Runtime.Diagnostics;

namespace IllusionScript.Runtime.Interface
{
    public class InterpreterResult
    {
        public readonly object value;
        public readonly DiagnosticGroup diagnostic;

        internal InterpreterResult(object value, DiagnosticGroup diagnostic)
        {
            this.value = value;
            this.diagnostic = diagnostic;
        }
    }
}