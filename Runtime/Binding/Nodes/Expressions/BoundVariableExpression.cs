using System;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    public sealed class BoundVariableExpression : BoundExpression
    {
        public readonly VariableSymbol variableSymbol;

        public BoundVariableExpression(VariableSymbol variableSymbol)
        {
            this.variableSymbol = variableSymbol;
        }

        public override BoundNodeType boundType => BoundNodeType.VariableExpression;
        public override TypeSymbol type => variableSymbol.type;
    }
}