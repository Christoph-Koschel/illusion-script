using System;
using System.Collections.Generic;
using System.Linq;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Memory
{
    internal sealed class BoundScope
    {
        public readonly BoundScope? parent;
        private Dictionary<string, Symbol>? symbols;

        public BoundScope(BoundScope? parent)
        {
            this.parent = parent;
        }

        public bool TryDeclareVariable(VariableSymbol symbol) => TryDeclareSymbol(symbol);

        private bool TryDeclareSymbol<T>(T symbol)
            where T : Symbol
        {
            if (symbols == null)
            {
                symbols = new Dictionary<string, Symbol>();
            }
            else if (symbols.ContainsKey(symbol.name))
            {
                return false;
            }

            symbols.Add(symbol.name, symbol);
            return true;
        }

        public Symbol? TryLookupSymbol(string name)
        {
            if (symbols != null && symbols.TryGetValue(name, out Symbol symbol))
            {
                return symbol;
            }

            return parent?.TryLookupSymbol(name);
        }

        public IEnumerable<VariableSymbol> GetDeclaredVariables() => GetDeclaredSymbols<VariableSymbol>();

        private IEnumerable<T> GetDeclaredSymbols<T>() where T : Symbol
        {
            if (symbols == null)
            {
                return Array.Empty<T>();
            }

            return symbols.Values.OfType<T>();
        }
    }
}