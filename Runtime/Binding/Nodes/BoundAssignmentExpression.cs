using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public readonly VariableSymbol variable;
        public readonly BoundExpression right;

        public BoundAssignmentExpression(Parsing.Nodes.Node node, VariableSymbol variable, BoundExpression right) :
            base(node)
        {
            this.variable = variable;
            this.right = right;
        }

        public override BoundNodeType boundType => BoundNodeType.AssignmentExpression;
        public override TypeSymbol type => right.type;
    }
}