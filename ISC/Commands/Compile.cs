using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IllusionScript.Runtime;
using IllusionScript.Runtime.Compiling;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.ISC.Commands
{
    public class Compile : CliController.CliCommand
    {
        public override string[] neededFlags => new[] { "-c" };
        public override string[] neededArguments => Array.Empty<string>();
        public override Dictionary<string, string> neededNamedArguments => new();

        public override int Exec(CliController controller)
        {
            if (controller.arguments.Length == 0)
            {
                Console.WriteLine("Needs at least on file to compile");
                return 1;
            }

            if (controller.namedArguments.ContainsKey("--target"))
            {
                string compiler = Compilation.GetCompilerHash(controller.namedArguments["--target"]);
                if (string.IsNullOrWhiteSpace(compiler))
                {
                    Console.WriteLine($"No compiler with the tag {controller.namedArguments["--target"]} found");
                    return 1;
                }

                Compilation compilation = CreateCompilation(controller);
                if (compilation == null)
                {
                    return 1;
                }

                string outDir = controller.namedArguments.ContainsKey("--out")
                    ? controller.namedArguments["--out"]
                    : Environment.CurrentDirectory;

                int type = controller.namedArguments.ContainsKey("--type")
                    ? CompilerConnector.DetectTarget(controller.namedArguments["--type"], CompilerConnector.Executable)
                    : CompilerConnector.Executable;

                return compilation.Compile(compiler, outDir, type, Console.Out) ? 0 : 1;
            }
            else
            {
                Compilation compilation = CreateCompilation(controller);
                if (compilation == null)
                {
                    return 1;
                }

                InterpreterResult result = compilation.Interpret(new Dictionary<VariableSymbol, object>());

                if (!result.diagnostics.Any())
                {
                    if (result.value == null)
                    {
                        return 0;
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(result.value);
                    Console.ResetColor();
                    return 0;
                }
                else
                {
                    Console.Out.WriteDiagnostics(result.diagnostics);
                    return 1;
                }
            }
        }

        private Compilation CreateCompilation(CliController controller)
        {
            IEnumerable<string> paths = GetFilePaths(controller.arguments);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            bool hasErrors = false;
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"ERROR: Path '{path}' is not a directory or file with the ending .ils");
                    hasErrors = true;
                    continue;
                }

                SyntaxTree syntaxTree = SyntaxTree.Load(path);
                syntaxTrees.Add(syntaxTree);
            }

            if (hasErrors)
            {
                return null;
            }

            Compilation compilation = Compilation.Create(syntaxTrees.ToArray());
            return compilation;
        }

        private IEnumerable<string> GetFilePaths(IEnumerable<string> paths)
        {
            SortedSet<string> result = new SortedSet<string>();

            foreach (string path in paths)
            {
                string target = Path.GetFullPath(path);

                if (Directory.Exists(target))
                {
                    result.UnionWith(Directory.EnumerateFiles(target, "*.ils", SearchOption.AllDirectories));
                }
                else
                {
                    result.Add(target);
                }
            }

            return result;
        }
    }
}