using System;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type type { get;  }
    }
}