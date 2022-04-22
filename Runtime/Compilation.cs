using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Lowering;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime
{
    public sealed class Compilation
    {
        public readonly Compilation previous;
        public readonly SyntaxTree syntaxTree;
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

            BoundStatement statement = GetStatement();
            Interpreter interpreter = new Interpreter(statement, variables);
            object value = interpreter.Interpret();

            return new InterpreterResult(Array.Empty<Diagnostic>(), value);
        }

        public void EmitTree(TextWriter writer)
        {
            BoundStatement expression = GetStatement();
            expression.WriteTo(writer);
        }

        private BoundStatement GetStatement()
        {
            BoundStatement result = GlobalScope.statement;
            return Lowerer.Lower(result);
        }
    }
}