using System;
using IllusionScript.Runtime.Binding.Operators;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public readonly BoundUnaryOperator unaryOperator;
        public readonly BoundExpression right;

        public BoundUnaryExpression(BoundUnaryOperator unaryOperator, BoundExpression right)
        {
            this.unaryOperator = unaryOperator;
            this.right = right;
        }

        public override Type type => unaryOperator.resultType;
        public override BoundNodeType boundType => BoundNodeType.UnaryExpression;
    }
}