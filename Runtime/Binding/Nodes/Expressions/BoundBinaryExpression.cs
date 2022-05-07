using System;
using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    public sealed class BoundBinaryExpression :BoundExpression
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
        public override TypeSymbol type => binaryOperator.resultType;
    }
}