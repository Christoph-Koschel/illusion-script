using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private readonly Dictionary<VariableSymbol, object> variables;

        public IlsRepl()
        {
            variables = new Dictionary<VariableSymbol, object>();
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            SyntaxTree syntaxTree = SyntaxTree.Parse(text);
            if (syntaxTree.diagnostics.Any())
            {
                return false;
            }

            return true;
        }

        protected override void Invoke(string input)
        {
            SyntaxTree syntaxTree = SyntaxTree.Parse(input);

            Compilation compilation = previous == null
                ? new Compilation(syntaxTree)
                : previous.ContinueWith(syntaxTree);

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
                SourceText text = syntaxTree.text;
                Console.Out.WriteDiagnostics(result.diagnostics, syntaxTree);
            }
        }

        protected override void InvokeMetaCommand(string lineInput)
        {
            switch (lineInput.ToLower())
            {
                case "#showtree":
                    showTree = !showTree;
                    Console.WriteLine($"$showTree has set to {showTree}");
                    break;
                case "#showprogram":
                    showProgram = !showProgram;
                    Console.WriteLine($"$showProgram has set to {showProgram}");
                    break;
                case "#cls":
                    Console.Clear();
                    Console.WriteLine("Console cleared");
                    break;
                case "#reset":
                    previous = null;
                    break;
                default:
                    base.InvokeMetaCommand(lineInput);
                    break;
            }
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
    }
}