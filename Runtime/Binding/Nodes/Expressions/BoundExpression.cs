using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    public abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol type { get;  }
    }
}