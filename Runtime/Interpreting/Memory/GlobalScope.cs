using System.Collections.Immutable;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    internal sealed class GlobalScope
    {
        public readonly GlobalScope previous;
        public readonly ImmutableArray<Diagnostic> diagnostics;
        public readonly FunctionSymbol mainFunction;
        public readonly FunctionSymbol scriptFunction;
        public readonly ImmutableArray<VariableSymbol> variables;
        public readonly ImmutableArray<FunctionSymbol> functions;
        public readonly ImmutableArray<BoundStatement> statements;

        public GlobalScope(GlobalScope previous, ImmutableArray<Diagnostic> diagnostics, FunctionSymbol mainFunction,
            FunctionSymbol scriptFunction,
            ImmutableArray<VariableSymbol> variables, ImmutableArray<FunctionSymbol> functions,
            ImmutableArray<BoundStatement> statements)
        {
            this.previous = previous;
            this.diagnostics = diagnostics;
            this.mainFunction = mainFunction;
            this.scriptFunction = scriptFunction;
            this.variables = variables;
            this.functions = functions;
            this.statements = statements;
        }
    }
}