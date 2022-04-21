using System;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    public sealed class VariableSymbol
    {
        public readonly string name;
        public readonly Type type;

        internal VariableSymbol(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }
}