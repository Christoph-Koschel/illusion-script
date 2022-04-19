using IllusionScript.Runtime.Binding;

namespace IllusionScript.Runtime.Symbols
{
    public abstract class VariableSymbol : Symbol
    {
        public readonly bool isReadOnly;
        public readonly TypeSymbol type;
        internal readonly BoundConstant? constant;

        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type, BoundConstant? constant) : base(name)
        {
            this.isReadOnly = isReadOnly;
            this.type = type;
            this.constant = isReadOnly ? constant : null;
        }
    }
}