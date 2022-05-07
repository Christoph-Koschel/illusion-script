using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionScript.Runtime;

namespace IllusionScript.ISC
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            CliController cliController = ParseArgs(args);

            if (!cliController.namedArguments.ContainsKey("--no-load"))
            {
                DetectCompilers();
            }

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

        private static void DetectCompilers()
        {
            if (Directory.Exists("etc"))
            {
                string[] dlls = Directory.EnumerateFiles("etc", "*.dll", SearchOption.AllDirectories).ToArray();
                foreach (string dll in dlls)
                {
                    Assembly assembly = Assembly.LoadFile(dll);
                    Compilation.AddCompiler(assembly);
                }
            }
        }

        private static CliController ParseArgs(string[] args)
        {
            List<string> flags = new List<string>();
            List<string> arguments = new List<string>();
            Dictionary<string, string> namedArguments = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                string current = args[i];
                string next = i + 1 < args.Length ? args[i + 1] : string.Empty;
                if (current.Length == 2 && current.StartsWith("-"))
                {
                    flags.Add(current);
                }
                else if (current.StartsWith("--"))
                {
                    string value;
                    if (next.StartsWith("-"))
                    {
                        value = string.Empty;
                    }
                    else
                    {
                        value = next;
                        i++;
                    }

                    namedArguments.Add(current, value);
                }
                else
                {
                    arguments.Add(current);
                }
            }

            return new CliController(flags.ToArray(), namedArguments, arguments.ToArray());
        }
    }
}