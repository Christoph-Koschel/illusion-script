using System;

namespace IllusionScript.Runtime.Binding.Node
{
    public abstract class BoundExpression : BoundNode
    {
        public abstract Type type { get; }
    }
}