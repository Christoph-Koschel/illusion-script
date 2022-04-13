using System;
using System.Linq;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interface;
using IllusionScript.Runtime.Lexing;

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

                Compilation compilation = Compilation.Parse(input);
                if (compilation.diagnostics.Any())
                {
                    foreach (Diagnostic diagnostic in compilation.diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                }
                else
                {
                    foreach (Token token in compilation.tokens)
                    {
                        Console.Write(token.type);

                        if (token.value != null)
                        {
                            Console.Write(" ");
                            Console.Write(token.value);
                        }
                        
                        Console.Write("\n");
                    }
                }
            }
        }
    }
}