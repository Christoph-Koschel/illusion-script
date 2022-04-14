using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Interface
{
    public sealed class Compilation
    {
        public readonly DiagnosticGroup diagnostics;
        public readonly SyntaxThree syntaxThree;

        private Compilation(DiagnosticGroup diagnostics, SyntaxThree syntaxThree)
        {
            this.diagnostics = diagnostics;
            this.syntaxThree = syntaxThree;
        }
    }
}