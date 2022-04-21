using System.Collections.Immutable;
using IllusionScript.Runtime.Binding.Nodes;
using IllusionScript.Runtime.Diagnostics;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    internal sealed class GlobalScope
    {
        public readonly GlobalScope previous;
        public readonly ImmutableArray<Diagnostic> diagnostics;
        public readonly ImmutableArray<VariableSymbol> variables;
        public readonly BoundExpression expression;

        public GlobalScope(GlobalScope previous, ImmutableArray<Diagnostic> diagnostics, ImmutableArray<VariableSymbol> variables, BoundExpression expression)
        {
            this.previous = previous;
            this.diagnostics = diagnostics;
            this.variables = variables;
            this.expression = expression;
        }
    }
}