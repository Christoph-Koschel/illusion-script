using System;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Node
{
    public abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol type { get; }
    }
}