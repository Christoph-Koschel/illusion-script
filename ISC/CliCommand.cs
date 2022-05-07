using System.Collections.Generic;
using System.Linq;

namespace IllusionScript.ISC
{
    public sealed partial class CliController
    {
        public abstract class CliCommand
        {
            public abstract string[] neededFlags { get; }
            public abstract string[] neededArguments { get; }
            public abstract Dictionary<string, string> neededNamedArguments { get; }

            public abstract int Exec(CliController controller);
            
            public bool IsCommand(CliController controller)
            {
                foreach (string flag in neededFlags)
                {
                    bool exists = controller.flags.Any(controllerFlag => flag == controllerFlag);

                    if (!exists)
                    {
                        return false;
                    }
                }

                foreach (string argument in neededArguments)
                {
                    bool exists = controller.arguments.Any(controllerArgument => argument == controllerArgument);

                    if (!exists)
                    {
                        return false;
                    }
                }

                foreach (KeyValuePair<string, string> namedArgument in neededNamedArguments)
                {
                    if (!controller.namedArguments.ContainsKey(namedArgument.Key))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}