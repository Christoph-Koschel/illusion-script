namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    internal sealed class LabelSymbol
    {
        public readonly string name;

        public LabelSymbol(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}