using System;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    public sealed class VariableSymbol
    {
        public readonly string name;
        public readonly bool isReadOnly;
        public readonly Type type;

        internal VariableSymbol(string name, bool isReadOnly, Type type)
        {
            this.name = name;
            this.isReadOnly = isReadOnly;
            this.type = type;
        }
    }
}