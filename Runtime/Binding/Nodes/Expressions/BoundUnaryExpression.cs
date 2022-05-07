using System;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    public sealed class BoundUnaryExpression : BoundExpression
    {
        public readonly BoundUnaryOperator unaryOperator;
        public readonly BoundExpression right;

        public BoundUnaryExpression(BoundUnaryOperator unaryOperator, BoundExpression right)
        {
            this.unaryOperator = unaryOperator;
            this.right = right;
        }

        public override TypeSymbol type => unaryOperator.resultType;
        public override BoundNodeType boundType => BoundNodeType.UnaryExpression;
    }
}