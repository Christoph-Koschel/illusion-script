using System.Reflection;

namespace IllusionScript.ISI
{
    internal abstract partial class Repl
    {
        private class MetaCommand
        {
            public readonly string name;
            public string description;
            public readonly MethodInfo method;

            public MetaCommand(string name, string description, MethodInfo method)
            {
                this.name = name;
                this.description = description;
                this.method = method;
            }
        }
    }
}