using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;
using ControlFlowGraph = IllusionScript.Runtime.CFA.ControlFlowGraph;

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

            BoundProgram program = Binder.BindProgram(GlobalScope);

            string appPath = Environment.GetCommandLineArgs()[0];
            string? appDirectory = Path.GetDirectoryName(appPath);
            string cfgPath = Path.Combine(appDirectory, "cfg.dot");
            BoundBlockStatement cfgStatement = !program.statement.statements.Any() && program.functionBodies.Any()
                ? program.functionBodies.Last().Value
                : program.statement;

            ControlFlowGraph cfg = ControlFlowGraph.Create(cfgStatement);
            using (StreamWriter streamWriter = new StreamWriter(cfgPath))
            {
                cfg.WriteTo(streamWriter);
            }

            if (program.diagnostics.Any())
            {
                return new InterpreterResult(program.diagnostics, null);
            }

            Interpreter interpreter = new Interpreter(program, variables);
            object value = interpreter.Interpret();

            return new InterpreterResult(Array.Empty<Diagnostic>(), value);
        }

        public void EmitTree(TextWriter writer)
        {
            BoundProgram program = Binder.BindProgram(GlobalScope);
            if (program.statement.statements.Any())
            {
                program.statement.WriteTo(writer);
            }
            else
            {
                foreach (KeyValuePair<FunctionSymbol, BoundBlockStatement> functionBody in program.functionBodies)
                {
                    if (!GlobalScope.functions.Contains(functionBody.Key))
                    {
                        continue;
                    }

                    functionBody.Key.WriteTo(writer);
                    functionBody.Value.WriteTo(writer);
                }
            }
        }
    }
}