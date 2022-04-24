namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public sealed class ParameterSymbol : VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type) : base(name, true, type)
        {
            
        }

        public override SymbolType symbolType => SymbolType.Parameter;
    }
}