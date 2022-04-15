using System;

namespace IllusionScript.Runtime.Binding.Node
{
    internal sealed class BoundLiteral : BoundExpression
    {
        public readonly object value;

        public BoundLiteral(object value)
        {
            this.value = value;
        }

        public override BoundNodeType boundType => BoundNodeType.LiteralExpression;
        public override Type type => value.GetType();
    }
}