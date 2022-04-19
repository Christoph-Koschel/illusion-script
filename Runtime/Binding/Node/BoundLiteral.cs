using System;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Node
{
    internal sealed class BoundLiteral : BoundExpression
    {
        public readonly object value;

        public BoundLiteral(object value)
        {
            this.value = value;
            type = value switch
            {
                int => TypeSymbol.Int,
                bool => TypeSymbol.Bool,
                string => TypeSymbol.String,
                _ => type
            };
        }

        public override BoundNodeType boundType => BoundNodeType.LiteralExpression;
        public override TypeSymbol type { get; }
    }
}