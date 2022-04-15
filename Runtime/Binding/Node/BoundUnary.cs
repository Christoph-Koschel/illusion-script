using System;
using IllusionScript.Runtime.Binding.Operators;

namespace IllusionScript.Runtime.Binding.Node
{
    internal sealed class BoundUnary : BoundExpression
    {
        public readonly BoundUnaryOperator unaryOperator;
        public readonly BoundExpression right;

        public BoundUnary(BoundUnaryOperator unaryOperator, BoundExpression right)
        {
            this.unaryOperator = unaryOperator;
            this.right = right;
        }

        public override BoundNodeType boundType => BoundNodeType.UnaryExpression;
        public override Type type => unaryOperator.resultType;
    }
}