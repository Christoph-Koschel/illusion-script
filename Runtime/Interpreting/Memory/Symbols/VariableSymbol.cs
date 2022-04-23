using System;

namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        public readonly bool isReadOnly;
        public readonly Type type;

        internal VariableSymbol(string name, bool isReadOnly, Type type) : base(name)
        {
            this.isReadOnly = isReadOnly;
            this.type = type;
        }

        public override string ToString()
        {
            return name;
        }

        public override SymbolType symbolType => SymbolType.Variable;
    }
}