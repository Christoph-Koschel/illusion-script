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
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;
using ControlFlowGraph = IllusionScript.Runtime.CFA.ControlFlowGraph;

namespace IllusionScript.Runtime
{
    public sealed class Compilation
    {
        public readonly Compilation previous;
        public readonly SyntaxTree[] syntaxTrees;
        private GlobalScope globalScope;

        public Compilation(params SyntaxTree[] syntaxThrees)
            : this(null, syntaxThrees)
        {
        }

        private Compilation(Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            this.previous = previous;
            this.syntaxTrees = syntaxTrees;
        }

        internal GlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    GlobalScope scope = Binder.BindGlobalScope(previous?.globalScope, syntaxTrees.ToImmutableArray());
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
            IEnumerable<Diagnostic> parseDiagnostics = syntaxTrees.SelectMany(st => st.diagnostics);
            ImmutableArray<Diagnostic> diagnostics = parseDiagnostics.Concat(GlobalScope.diagnostics).ToImmutableArray();
            
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

            try
            {
                ControlFlowGraph cfg = ControlFlowGraph.Create(cfgStatement);
                using StreamWriter streamWriter = new StreamWriter(cfgPath);
                cfg.WriteTo(streamWriter);
            }
            catch (Exception err)
            {
                if (err is not UnauthorizedAccessException)
                {
                    throw err;
                }
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