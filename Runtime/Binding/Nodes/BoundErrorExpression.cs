using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public BoundErrorExpression(Parsing.Nodes.Node node) : base(node)
        {
        }

        public override BoundNodeType boundType => BoundNodeType.ErrorExpression;
        public override TypeSymbol type => TypeSymbol.Error;
    }
}