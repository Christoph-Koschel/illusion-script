using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IllusionScript.Runtime;
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
                Console.WriteLine("usage: ils <source-paths>");
                return;
            }

            IEnumerable<string> paths = GetFilePaths(args);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            var hasErrors = false;
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"ERROR: File '{path}' doesn't exists");
                    hasErrors = true;
                    continue;
                }

                SyntaxTree syntaxTree = SyntaxTree.Load(path);
                syntaxTrees.Add(syntaxTree);
            }

            if (hasErrors)
            {
                return;
            }

            Compilation compilation = new Compilation(syntaxTrees.ToArray());
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
                Console.Out.WriteDiagnostics(result.diagnostics);
            }
        }

        private static IEnumerable<string> GetFilePaths(IEnumerable<string> paths)
        {
            var result = new SortedSet<string>();

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    result.UnionWith(Directory.EnumerateFiles(path, "*.ils", SearchOption.AllDirectories));
                }
                else
                {
                    result.Add(path);
                }
            }

            return result;
        }
    }
}