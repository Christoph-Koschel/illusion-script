namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public abstract class Symbol
    {
        public readonly string name;

        internal Symbol(string name)
        {
            this.name = name;
        }

        public abstract SymbolType symbolType { get; }

        public override string ToString()
        {
            return name;
        }
    }
}