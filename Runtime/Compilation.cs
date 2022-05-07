using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Compiling;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;
using Binder = IllusionScript.Runtime.Binding.Binder;

namespace IllusionScript.Runtime
{
    public sealed class Compilation
    {
        public static readonly Dictionary<string, CompilerConnector> compilers =
            new();

        public readonly bool isScript;
        public readonly Compilation previous;
        public readonly ImmutableArray<SyntaxTree> syntaxTrees;
        public ImmutableArray<FunctionSymbol> functions => GlobalScope.functions;
        public ImmutableArray<VariableSymbol> variables => GlobalScope.variables;
        public FunctionSymbol mainFunction => globalScope.mainFunction;
        private GlobalScope globalScope;

        private Compilation(bool isScript, Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            this.isScript = isScript;
            this.previous = previous;
            this.syntaxTrees = syntaxTrees.ToImmutableArray();
        }

        public static Compilation Create(params SyntaxTree[] syntaxTrees)
        {
            return new Compilation(false, null, syntaxTrees);
        }

        public static Compilation CreateScript(Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            return new Compilation(true, previous, syntaxTrees);
        }

        public static void AddCompiler(Assembly assembly)
        {
            Type baseType = typeof(CompilerConnector);
            Type[] types = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();

            foreach (Type type in types)
            {
                CompilerConnector compiler = (CompilerConnector)Activator.CreateInstance(type);
                if (compiler != null)
                {
                    compilers.Add(compiler.hashID, compiler);
                }
            }
        }

        public static string GetCompilerHash(string name)
        {
            CompilerConnector compiler = compilers.FirstOrDefault(c => c.Value.name == name).Value;

            return compiler?.hashID;
        }

        internal GlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    GlobalScope scope = Binder.BindGlobalScope(isScript, previous?.globalScope,
                        syntaxTrees.ToImmutableArray());
                    Interlocked.CompareExchange(ref globalScope, scope, null);
                }

                return globalScope;
            }
        }

        private BoundProgram GetProgram()
        {
            BoundProgram previous = this.previous == null ? null : this.previous.GetProgram();
            return Binder.BindProgram(isScript, previous, GlobalScope);
        }

        public IEnumerable<Symbol> GetSymbols()
        {
            Compilation compilation = this;
            HashSet<string> seenSymbols = new HashSet<string>();
            while (compilation != null)
            {
                foreach (FunctionSymbol function in compilation.functions)
                {
                    if (seenSymbols.Add(function.name))
                    {
                        yield return function;
                    }
                }

                foreach (VariableSymbol variable in compilation.variables)
                {
                    if (seenSymbols.Add(variable.name))
                    {
                        yield return variable;
                    }
                }

                compilation = compilation.previous;
            }
        }

        public InterpreterResult Interpret(Dictionary<VariableSymbol, object> variables)
        {
            IEnumerable<Diagnostic> parseDiagnostics = syntaxTrees.SelectMany(st => st.diagnostics);
            ImmutableArray<Diagnostic>
                diagnostics = parseDiagnostics.Concat(GlobalScope.diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                return new InterpreterResult(diagnostics, null);
            }

            BoundProgram program = GetProgram();

            if (program.diagnostics.Any())
            {
                return new InterpreterResult(program.diagnostics, null);
            }

            Interpreter interpreter = new Interpreter(program, variables);
            object value = interpreter.Interpret();

            return new InterpreterResult(Array.Empty<Diagnostic>(), value);
        }

        public bool Compile(string id, string output, TextWriter writer)
        {
            if (!compilers.TryGetValue(id, out CompilerConnector compiler))
            {
                return false;
            }

            output = Path.GetFullPath(output);

            if (!Directory.Exists(output))
            {
                writer.WriteLine($"'{output}' does not exists");
            }

            compiler.setBaseDir(output);
            if (!compiler.BuildOutput())
            {
                writer.WriteLine("Failed to create Output (See errors above)");
                return false;
            }

            if (!compiler.Build(GetProgram()))
            {
                writer.WriteLine("Failed to Build (See errors above)");
                return false;
            }

            if (!compiler.CleanUp())
            {
                writer.WriteLine("Cleanup failed (See errors above)");
                return false;
            }

            writer.WriteLine("Build succeeded");
            return true;
        }

        public void EmitTree(TextWriter writer)
        {
            if (GlobalScope.mainFunction != null)
            {
                EmitTree(GlobalScope.mainFunction, writer);
            }
            else if (GlobalScope.scriptFunction != null)
            {
                EmitTree(GlobalScope.scriptFunction, writer);
            }
        }

        public void EmitTree(FunctionSymbol symbol, TextWriter writer)
        {
            BoundProgram program = GetProgram();
            if (!program.functionBodies.TryGetValue(symbol, out BoundBlockStatement body))
            {
                return;
            }

            symbol.WriteTo(writer);
            writer.Write("\n");
            body.WriteTo(writer);
        }
    }
}