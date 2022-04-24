using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public readonly BoundExpression expression;

        public BoundConversionExpression(TypeSymbol type, BoundExpression expression)
        {
            this.expression = expression;
            this.type = type;
        }
        
        public override BoundNodeType boundType => BoundNodeType.ConversionExpression;
        public override TypeSymbol type { get; }
    }
}