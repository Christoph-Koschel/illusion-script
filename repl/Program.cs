using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IllusionScript.Runtime;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            bool showTree = true;
            bool showProgram = true;
            StringBuilder textBuilder = new StringBuilder();
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
            Compilation previous = null;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(textBuilder.Length == 0 ? "» " : "· ");
                Console.ResetColor();

                string lineInput = Console.ReadLine();
                bool isBlank = string.IsNullOrWhiteSpace(lineInput);


                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }

                    switch (lineInput.ToLower())
                    {
                        case "#showtree":
                            showTree = !showTree;
                            Console.WriteLine($"$showTree has set to {showTree}");
                            continue;
                        case "#showprogram":
                            showProgram = !showProgram;
                            Console.WriteLine($"$showProgram has set to {showProgram}");
                            continue;
                        case "#cls":
                            Console.Clear();
                            Console.WriteLine("Console cleared");
                            continue;
                        case "#reset":
                            previous = null;
                            continue;
                    }
                }

                textBuilder.AppendLine(lineInput);
                string input = textBuilder.ToString();

                SyntaxTree syntaxThree = SyntaxTree.Parse(input);

                if (!isBlank && syntaxThree.diagnostics.Any())
                {
                    continue;
                }

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

                textBuilder.Clear();
            }
        }
    }
}