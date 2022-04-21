using System;
using IllusionScript.Runtime.Interpreting.Memory;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public readonly VariableSymbol variableSymbol;

        public BoundVariableExpression(VariableSymbol variableSymbol)
        {
            this.variableSymbol = variableSymbol;
        }

        public override BoundNodeType boundType => BoundNodeType.VariableExpression;
        public override Type type => variableSymbol.type;
    }
}