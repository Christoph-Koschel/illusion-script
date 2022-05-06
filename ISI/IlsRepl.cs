using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using IllusionScript.Runtime;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.ISI
{
    internal sealed class IlsRepl : Repl
    {
        private Compilation previous;
        private bool showTree = true; // <<-- TODO remove true
        private bool showProgram = true; // <<-- TODO remove true
        private bool loadingSubmission;
        private readonly Dictionary<VariableSymbol, object> variables;

        public IlsRepl()
        {
            variables = new Dictionary<VariableSymbol, object>();
            LoadCompilations();
        }

        protected override void Invoke(string input)
        {
            SyntaxTree syntaxTree = SyntaxTree.Parse(input, "<stdin>");

            Compilation compilation = Compilation.CreateScript(previous, syntaxTree);

            previous = compilation;

            if (showTree)
            {
                syntaxTree.root.WriteTo(Console.Out);
                Console.Write("\n");

                Console.ResetColor();
            }

            if (showProgram)
            {
                compilation.EmitTree(Console.Out);
                Console.Write("\n");
            }

            InterpreterResult result = compilation.Interpret(variables);

            if (!result.diagnostics.Any())
            {
                SaveCompilation(input);
                if (result.value == null)
                {
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(result.value);
                Console.ResetColor();
            }
            else
            {
                Console.Out.WriteDiagnostics(result.diagnostics);
            }
        }

        private void SaveCompilation(string input)
        {
            if (loadingSubmission)
            {
                return;
            }

            string compilationFolder = GetCompilationPath();
            Directory.CreateDirectory(compilationFolder);
            int count = Directory.GetFiles(compilationFolder).Length;
            string name = $"compilation{count:0000}";
            string fileName = Path.Combine(compilationFolder, name);
            File.WriteAllText(fileName, input);
        }

        private static string GetCompilationPath()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string compilationFolder = Path.Combine(localAppData, "IllusionScript", "Compilations");
            return compilationFolder;
        }

        private void ClearCompilations()
        {
            string path = GetCompilationPath();
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private void LoadCompilations()
        {
            string compilationFolder = GetCompilationPath();
            if (!Directory.Exists(compilationFolder))
            {
                return;
            }

            string[] files = Directory.GetFiles(compilationFolder).OrderBy(f => f).ToArray();
            if (files.Length == 0)
            {
                return;
            }

            loadingSubmission = true;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Loaded {files.Length} submission(s)");
            Console.ResetColor();

            foreach (string file in files)
            {
                string text = File.ReadAllText(file);
                Invoke(text);
            }

            loadingSubmission = false;
        }

        protected override void Renderer(string line)
        {
            if (Program.args.Contains("-d"))
            {
                base.Renderer(line);
                return;
            }

            IEnumerable<Token> tokens = SyntaxTree.ParseTokens(line, out ImmutableArray<Diagnostic> diagnostics);

            foreach (Token token in tokens)
            {
                if (token.type.ToString().EndsWith("Keyword"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (token.type == SyntaxType.IdentifierToken)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (token.type == SyntaxType.StringToken)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (token.type == SyntaxType.NumberToken)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.Write(token.text);
                Console.ResetColor();
            }
        }

        [MetaCommand("showtree", "Shows the parse tree")]
        private void InvokeShowTree()
        {
            showTree = !showTree;
            Console.WriteLine($"$showTree has set to {showTree}");
        }

        [MetaCommand("showprogram", "Shows the program tree")]
        private void InvokeShowProgram()
        {
            showProgram = !showProgram;
            Console.WriteLine($"$showProgram has set to {showProgram}");
        }


        [MetaCommand("cls", "Clears the console")]
        private void InvokeCLS()
        {
            Console.Clear();
            Console.WriteLine("Console cleared");
        }

        [MetaCommand("reset", "Reset all compilations")]
        private void InvokeReset()
        {
            previous = null;
            variables.Clear();
            ClearCompilations();
        }

        [MetaCommand("load", "Loads a script file")]
        private void InvokeLoad(string path)
        {
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"ERROR File does not exists '{path}'");
                Console.ResetColor();
                return;
            }

            string text = File.ReadAllText(path);
            Invoke(text);
        }

        [MetaCommand("ls", "List all symbols")]
        private void InvokeLS()
        {
            if (previous == null)
            {
                return;
            }

            IOrderedEnumerable<Symbol> symbols = previous.GetSymbols().OrderBy(s => s.symbolType).ThenBy(s => s.name);
            foreach (Symbol symbol in symbols)
            {
                symbol.WriteTo(Console.Out);
                Console.Write("\n");
            }
        }

        [MetaCommand("dump", "Shows program tree of given function name")]
        private void InvokeDump(string name)
        {
            if (previous == null)
            {
                return;
            }

            FunctionSymbol? symbol =
                previous.GetSymbols().OfType<FunctionSymbol>().SingleOrDefault(f => f.name == name);
            if (symbol == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"ERROR Function '{name}' does not exists");
                Console.ResetColor();
                return;
            }

            previous.EmitTree(symbol, Console.Out);
        }
    }
}