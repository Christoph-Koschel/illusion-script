using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    internal sealed class Scope
    {
        public readonly Scope parent;
        private Dictionary<string, VariableSymbol> variables;
        private Dictionary<string, FunctionSymbol> functions;

        public Scope(Scope parent)
        {
            this.parent = parent;
            variables = new Dictionary<string, VariableSymbol>();
            functions = new Dictionary<string, FunctionSymbol>();
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            if (variables.TryGetValue(name, out variable))
            {
                return true;
            }

            if (parent == null)
            {
                return false;
            }

            return parent.TryLookupVariable(name, out variable);
        }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (variables.ContainsKey(variable.name))
            {
                return false;
            }

            variables.Add(variable.name, variable);
            return true;
        }
        
        public bool TryLookupFunction(string name, out FunctionSymbol variable)
        {
            if (functions.TryGetValue(name, out variable))
            {
                return true;
            }

            if (parent == null)
            {
                return false;
            }

            return parent.TryLookupFunction(name, out variable);
        }

        public bool TryDeclareFunction(FunctionSymbol variable)
        {
            if (functions.ContainsKey(variable.name))
            {
                return false;
            }

            functions.Add(variable.name, variable);
            return true;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variables.Values.ToImmutableArray();
        }
        
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            return functions.Values.ToImmutableArray();
        }
    }
}