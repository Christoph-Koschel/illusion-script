using System.Collections.Generic;
using System.Collections.Immutable;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    internal sealed class Scope
    {
        public readonly Scope parent;
        private Dictionary<string, VariableSymbol> variableSymbols;

        public Scope(Scope parent)
        {
            this.parent = parent;
            variableSymbols = new Dictionary<string, VariableSymbol>();
        }

        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if (variableSymbols.TryGetValue(name, out variable))
            {
                return true;
            }

            if (parent == null)
            {
                return false;
            }

            return parent.TryLookup(name, out variable);
        }

        public bool TryDeclare(VariableSymbol variable)
        {
            if (variableSymbols.ContainsKey(variable.name))
            {
                return false;
            }

            variableSymbols.Add(variable.name, variable);
            return true;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variableSymbols.Values.ToImmutableArray();
        }
    }
}