using System.Collections.Immutable;

namespace IllusionScript.Runtime.Interpreting.Memory.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public readonly ImmutableArray<ParameterSymbol> parameters;
        public readonly TypeSymbol returnType;

        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType) :
            base(name)
        {
            this.parameters = parameters;
            this.returnType = returnType;
        }

        public override SymbolType symbolType => SymbolType.Function;
    }
}