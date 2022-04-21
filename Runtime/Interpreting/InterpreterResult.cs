using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Diagnostics;

namespace IllusionScript.Runtime.Interpreting
{
    public sealed class InterpreterResult
    {
        public readonly ImmutableArray<Diagnostic> diagnostics;
        public readonly object value;
        
        public InterpreterResult(IEnumerable<Diagnostic> diagnostics, object value)
        {
            this.diagnostics = diagnostics.ToImmutableArray();
            this.value = value;
        }
    }
}