using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime
{
    public sealed class Compilation
    {
        private readonly Compilation previous;
        private readonly SyntaxTree syntaxTree;
        private GlobalScope globalScope;

        public Compilation(SyntaxTree syntaxThree)
            : this(null, syntaxThree)
        {
        }

        private Compilation(Compilation previous, SyntaxTree syntaxTree)
        {
            this.previous = previous;
            this.syntaxTree = syntaxTree;
        }

        internal GlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    GlobalScope scope = Binder.BindGlobalScope(previous?.globalScope, syntaxTree.root);
                    Interlocked.CompareExchange(ref globalScope, scope, null);
                }

                return globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxThree)
        {
            return new Compilation(this, syntaxThree);
        }

        public InterpreterResult Interpret(Dictionary<VariableSymbol, object> variables)
        {
            ImmutableArray<Diagnostic> diagnostics =
                syntaxTree.diagnostics.Concat(GlobalScope.diagnostics).ToImmutableArray();
            if (diagnostics.Any())
            {
                return new InterpreterResult(diagnostics, null);
            }

            Interpreter interpreter = new Interpreter(GlobalScope.expression, variables);
            object value = interpreter.Interpret();

            return new InterpreterResult(Array.Empty<Diagnostic>(), value);
        }
    }
}