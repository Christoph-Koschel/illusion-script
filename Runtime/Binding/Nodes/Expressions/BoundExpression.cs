﻿using System;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol type { get;  }
    }
}