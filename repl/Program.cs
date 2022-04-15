using System;
using System.Linq;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interface;
using IllusionScript.Runtime.Parsing;

namespace repl
{
    internal sealed class Program
    {
        static void Main(string[] args)
        {
            bool showTree = true; // TODO change on release to false;

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input.ToLower() == "#showtree")
                {
                    showTree = !showTree;
                    Console.WriteLine($"#ShowThree has set to {showTree}");
                    continue;
                }
                else if (input.ToLower() == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                Compilation compilation = SyntaxThree.Parse(input);
                if (compilation.diagnostics.Any())
                {
                    foreach (Diagnostic diagnostic in compilation.diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                }
                else
                {
                    if (showTree)
                    {
                        compilation.syntaxThree.root.WriteTo(Console.Out);
                    }

                    InterpreterResult result = compilation.Interpret();
                    if (result.diagnostic.Any())
                    {
                        foreach (Diagnostic diagnostic in result.diagnostic)
                        {
                            Console.WriteLine(diagnostic);
                        }
                    }
                    else
                    {
                        Console.WriteLine(result.value);
                    }
                }
            }
        }
    }
}