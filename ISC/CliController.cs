using System;
using System.Collections.Generic;

namespace IllusionScript.ISC
{
    public sealed partial class CliController
    {
        public readonly string[] flags;
        public readonly Dictionary<string, string> namedArguments;
        public readonly string[] arguments;
        private readonly List<CliCommand> commands;

        public CliController(string[] flags, Dictionary<string, string> namedArguments, string[] arguments)
        {
            this.flags = flags;
            this.namedArguments = namedArguments;
            this.arguments = arguments;
            commands = new List<CliCommand>();
        }

        public void Register(CliCommand command)
        {
            commands.Add(command);
        }

        public int Exec()
        {
            foreach (CliCommand command in commands)
            {
                if (command.IsCommand(this))
                {
                    return command.Exec(this);
                }
            }

            Console.WriteLine($"No command found who match the parameters");
            return 1;
        }
    }
}