using System;
using System.Linq;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;

namespace repl
{
    internal sealed class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                SyntaxThree syntaxThree = SyntaxThree.Parse(input);
                if (syntaxThree.diagnostics.Any())
                {
                    foreach (Diagnostic diagnostic in syntaxThree.diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                }
                else
                {
                    syntaxThree.root.WriteTo(Console.Out);
                }
            }
        }
    }
}