using System;
using System.Collections.Generic;
using System.Linq;
using IllusionScript.Runtime;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript
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
            SyntaxTree syntaxThree = SyntaxTree.Parse(input);

            Compilation compilation = previous == null
                ? new Compilation(syntaxThree)
                : previous.ContinueWith(syntaxThree);

            previous = compilation;

            if (showTree)
            {
                syntaxThree.root.WriteTo(Console.Out);
                Console.Write("\n");

                Console.ResetColor();
            }

            if (showProgram)
            {
                compilation.EmitTree(Console.Out);
            }

            InterpreterResult result = compilation.Interpret(variables);

            if (!result.diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(result.value);
                Console.ResetColor();

                previous = compilation;
            }
            else
            {
                SourceText text = syntaxThree.text;

                foreach (Diagnostic diagnostic in result.diagnostics)
                {
                    int lineIndex = text.GetLineIndex(diagnostic.span.start);
                    // TextLine line = text.lines[lineIndex];
                    // int lineNumber = lineIndex + 1;
                    // int character = diagnostic.span.start - line.start + 1;
                    //
                    // Console.ForegroundColor = ConsoleColor.DarkRed;
                    // Console.Write($"({lineNumber}, {character}): ");
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    // TextSpan prefixSpan = TextSpan.FromBounds(line.start, diagnostic.span.start);
                    // TextSpan suffixSpan = TextSpan.FromBounds(diagnostic.span.end, line.end);
                    //
                    // string prefix = syntaxThree.text.ToString(prefixSpan);
                    // string error = syntaxThree.text.ToString(diagnostic.span);
                    // string suffix = syntaxThree.text.ToString(suffixSpan);
                    //
                    // Console.Write(prefix);
                    //
                    // Console.ForegroundColor = ConsoleColor.DarkRed;
                    // Console.Write(error);
                    // Console.ResetColor();
                    //
                    // Console.Write(suffix);
                    Console.Write("\n\n");
                }
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

            IEnumerable<Token> tokens = SyntaxTree.ParseTokens(line);

            foreach (Token token in tokens)
            {
                if (token.type.ToString().EndsWith("Keyword"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
                else if (token.type == SyntaxType.IdentifierToken)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (token.type != SyntaxType.NumberToken)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }

                Console.Write(token.text);
                Console.ResetColor();
            }
        }
    }
}