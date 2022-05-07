using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionScript.Runtime;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.ISC
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            CliController cliController = ParseArgs(args);

            Type baseType = typeof(CliController.CliCommand);
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t =>
                baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).ToArray();

            foreach (Type type in types)
            {
                CliController.CliCommand command = (CliController.CliCommand)Activator.CreateInstance(type);

                cliController.Register(command);
            }

            return cliController.Exec();
        }

        private static CliController ParseArgs(string[] args)
        {
            List<string> flags = new List<string>();
            List<string> arguments = new List<string>();
            Dictionary<string, string> namedArguments = new Dictionary<string, string>();

            for (var i = 0; i < args.Length; i++)
            {
                string current = args[i];
                string next = i + 1 < args.Length ? args[i + 1] : string.Empty;
                if (current.Length == 2 && current.StartsWith("-"))
                {
                    flags.Add(current);
                }
                else if (current.StartsWith("--"))
                {
                    i++;
                    namedArguments.Add(current, next);
                }
                else
                {
                    arguments.Add(current);
                }
            }

            return new CliController(flags.ToArray(), namedArguments, arguments.ToArray());
        }

        // private static int Main(string[] args)
        // {
        //     if (args.Length == 0)
        //     {
        //         Console.WriteLine("usage: ils <source-paths>");
        //         return 1;
        //     }
        //
        //     IEnumerable<string> paths = GetFilePaths(args);
        //     List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
        //     bool hasErrors = false;
        //     foreach (string path in paths)
        //     {
        //         if (!File.Exists(path))
        //         {
        //             Console.WriteLine($"ERROR: File '{path}' doesn't exists");
        //             hasErrors = true;
        //             continue;
        //         }
        //
        //         SyntaxTree syntaxTree = SyntaxTree.Load(path);
        //         syntaxTrees.Add(syntaxTree);
        //     }
        //
        //     if (hasErrors)
        //     {
        //         return 1;
        //     }
        //
        //     Compilation compilation = Compilation.Create(syntaxTrees.ToArray());
        //     InterpreterResult result = compilation.Interpret(new Dictionary<VariableSymbol, object>());
        //
        //     if (!result.diagnostics.Any())
        //     {
        //         if (result.value == null)
        //         {
        //             return 0;
        //         }
        //
        //         Console.ForegroundColor = ConsoleColor.Gray;
        //         Console.WriteLine(result.value);
        //         Console.ResetColor();
        //         return 0;
        //     }
        //     else
        //     {
        //         Console.Out.WriteDiagnostics(result.diagnostics);
        //         return 1;
        //     }
        // }

        private static IEnumerable<string> GetFilePaths(IEnumerable<string> paths)
        {
            SortedSet<string> result = new SortedSet<string>();

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