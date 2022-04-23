namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Int = new TypeSymbol("Int");
        public static readonly TypeSymbol Bool = new TypeSymbol("Boolean");
        public static readonly TypeSymbol String = new TypeSymbol("String");
        public static readonly TypeSymbol Error = new TypeSymbol("?");

        private TypeSymbol(string name) : base(name)
        {
        }

        public override SymbolType symbolType => SymbolType.Type;
    }
}