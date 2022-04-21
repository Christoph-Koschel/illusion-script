using System;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public readonly object value;

        public BoundLiteralExpression(object value)
        {
            this.value = value;
        }

        public override BoundNodeType boundType => BoundNodeType.LiteralExpression;
        public override Type type => value.GetType();
    }
}