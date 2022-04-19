using IllusionScript.Runtime.Binding.Operators;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal sealed class BoundCompoundAssignmentExpression : BoundExpression
    {
        public readonly VariableSymbol variable;
        public readonly BoundBinaryOperator op;
        public readonly BoundExpression right;

        public BoundCompoundAssignmentExpression(Parsing.Nodes.Node node, VariableSymbol variable,
            BoundBinaryOperator op, BoundExpression right) : base(node)
        {
            this.variable = variable;
            this.op = op;
            this.right = right;
        }

        public override BoundNodeType boundType => BoundNodeType.CompoundAssignmentExpression;
        public override TypeSymbol type => right.type;
    }
}