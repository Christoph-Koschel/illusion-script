﻿using System.Collections.Immutable;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class BoundProgram
    {
        public readonly BoundProgram previous;
        public readonly GlobalScope globalScope;
        public readonly DiagnosticGroup diagnostics;
        public readonly ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies;
        public readonly BoundBlockStatement statement;

        public BoundProgram(BoundProgram previous, GlobalScope globalScope, DiagnosticGroup diagnostics, ImmutableDictionary<FunctionSymbol,BoundBlockStatement> functionBodies, BoundBlockStatement statement)
        {
            this.previous = previous;
            this.globalScope = globalScope;
            this.diagnostics = diagnostics;
            this.functionBodies = functionBodies;
            this.statement = statement;
        }
    }
}