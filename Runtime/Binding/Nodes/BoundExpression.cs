using IllusionScript.Runtime.Binding.Node;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes
{
    public abstract class BoundExpression : BoundNode
    {


        protected BoundExpression(Parsing.Nodes.Node node) : base(node)
        {
        }
        
        public abstract TypeSymbol type { get; }
    }
}