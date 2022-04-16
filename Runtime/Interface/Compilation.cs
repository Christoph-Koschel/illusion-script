using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Node;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Interface
{
    public sealed class Compilation
    {
        public readonly DiagnosticGroup diagnostics;
        public readonly SyntaxTree SyntaxTree;

        internal Compilation(DiagnosticGroup diagnostics, SyntaxTree syntaxTree)
        {
            this.diagnostics = diagnostics;
            this.SyntaxTree = syntaxTree;
        }

        public InterpreterResult Interpret()
        {
            Binder binder = new Binder();
            BoundExpression expression = binder.Bind(SyntaxTree.root);

            Interpreter interpreter = new Interpreter(expression);
            object result = interpreter.Interpret();
            binder.diagnostics.AddRange(interpreter.diagnostic);

            return new InterpreterResult(result, binder.diagnostics);
        }
    }
}