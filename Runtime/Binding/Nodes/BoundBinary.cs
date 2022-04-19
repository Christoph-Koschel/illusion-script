using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal sealed class BoundBinary : BoundExpression
    {
        public readonly BoundExpression left;
        public readonly BoundBinaryOperator binaryOperator;
        public readonly BoundExpression right;

        public BoundBinary(Parsing.Nodes.Node node, BoundExpression left, BoundBinaryOperator binaryOperator, BoundExpression right) : base(node)
        {
            this.left = left;
            this.binaryOperator = binaryOperator;
            this.right = right;
        }

        public override BoundNodeType boundType => BoundNodeType.BinaryExpression;
        public override TypeSymbol type => binaryOperator.resultType;
    }
}