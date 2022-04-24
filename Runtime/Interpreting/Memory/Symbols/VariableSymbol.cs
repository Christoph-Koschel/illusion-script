using System;

namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public class VariableSymbol : Symbol
    {
        public readonly bool isReadOnly;
        public readonly TypeSymbol type;

        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type) : base(name)
        {
            this.isReadOnly = isReadOnly;
            this.type = type;
        }

        public override SymbolType symbolType => SymbolType.Variable;
    }
}