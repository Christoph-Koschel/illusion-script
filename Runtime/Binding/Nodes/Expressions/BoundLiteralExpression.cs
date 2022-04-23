using System;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public readonly object value;

        public BoundLiteralExpression(object value)
        {
            if (value is int)
            {
                type = TypeSymbol.Int;
            }
            else if (value is bool)
            {
                type = TypeSymbol.Bool;
            }
            else if (value is string)
            {
                type = TypeSymbol.String;
            }
            else
            {
                throw new Exception($"Unexpected literal '{value}' of type {value.GetType()}");
            }
            
            this.value = value;
        }

        public override BoundNodeType boundType => BoundNodeType.LiteralExpression;
        public override TypeSymbol type { get; }
    }
}