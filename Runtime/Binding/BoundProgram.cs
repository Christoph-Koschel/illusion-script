using System.Collections.Immutable;
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
        public readonly FunctionSymbol mainFunction;
        public readonly FunctionSymbol scriptFunction;
        public readonly ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies;

        public BoundProgram(
            BoundProgram previous, 
            GlobalScope globalScope, 
            DiagnosticGroup diagnostics, 
            FunctionSymbol mainFunction,
            FunctionSymbol scriptFunction,
            ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies
            )
        {
            this.previous = previous;
            this.globalScope = globalScope;
            this.diagnostics = diagnostics;
            this.mainFunction = mainFunction;
            this.scriptFunction = scriptFunction;
            this.functionBodies = functionBodies;
        }
    }
}