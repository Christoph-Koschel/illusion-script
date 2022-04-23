namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public abstract class Symbol
    {
        public readonly string name;

        private protected Symbol(string name)
        {
            this.name = name;
        }
        
        public abstract SymbolType symbolType { get; }
    }
}