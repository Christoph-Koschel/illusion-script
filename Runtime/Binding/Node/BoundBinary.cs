using System;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Node
{
    internal sealed class BoundBinary : BoundExpression
    {
        public readonly BoundExpression left;
        public readonly BoundBinaryOperator binaryOperator;
        public readonly BoundExpression right;

        public BoundBinary(BoundExpression left, BoundBinaryOperator binaryOperator, BoundExpression right)
        {
            this.left = left;
            this.binaryOperator = binaryOperator;
            this.right = right;
        }

        public override BoundNodeType boundType => BoundNodeType.BinaryExpression;
        public override TypeSymbol type => binaryOperator.resultType;
    }
}