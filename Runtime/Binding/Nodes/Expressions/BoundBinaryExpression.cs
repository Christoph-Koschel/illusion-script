using System;
using IllusionScript.Runtime.Binding.Operators;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal sealed class BoundBinaryExpression :BoundExpression
    {
        public readonly BoundExpression left;
        public readonly BoundBinaryOperator binaryOperator;
        public readonly BoundExpression right;

        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator binaryOperator, BoundExpression right)
        {
            this.left = left;
            this.binaryOperator = binaryOperator;
            this.right = right;
        }


        public override BoundNodeType boundType => BoundNodeType.BinaryExpression;
        public override Type type => binaryOperator.resultType;
    }
}