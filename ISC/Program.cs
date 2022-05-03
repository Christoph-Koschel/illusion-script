using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IllusionScript.Runtime;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.ISC
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("usage: ils <source-paths>");
                return;
            }

            if (args.Length > 1)
            {
                Console.WriteLine("error: only one path supported right now");
                return;
            }

            var path = args.Single();
            var fileContent = File.ReadAllText(path);


            SyntaxTree syntaxTree = SyntaxTree.Parse(fileContent);

            Compilation compilation = new Compilation(syntaxTree);
            InterpreterResult result = compilation.Interpret(new Dictionary<VariableSymbol, object>());

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
    }
}