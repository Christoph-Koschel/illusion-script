using System;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type type { get;  }
    }
}