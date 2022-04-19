using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public readonly BoundExpression expression;

        public BoundConversionExpression(Parsing.Nodes.Node node, TypeSymbol type, BoundExpression expression) : base(node)
        {
            this.expression = expression;
            this.type = type;
        }

        public override BoundNodeType boundType => BoundNodeType.ConversionExpression;
        public override TypeSymbol type { get; }
    }
}