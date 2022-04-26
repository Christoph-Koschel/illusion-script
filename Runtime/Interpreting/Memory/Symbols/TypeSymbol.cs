namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Int = new ("Int");
        public static readonly TypeSymbol Bool = new ("Boolean");
        public static readonly TypeSymbol String = new ("String");
        public static readonly TypeSymbol Void = new ("Void");
        public static readonly TypeSymbol Error = new ("?");

        private TypeSymbol(string name) : base(name)
        {
        }

        public override SymbolType symbolType => SymbolType.Type;
    }
}