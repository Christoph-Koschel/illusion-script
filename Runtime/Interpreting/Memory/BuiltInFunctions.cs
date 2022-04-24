using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Interpreting.Memory
{
    internal static class BuiltInFunctions
    {
        public static readonly FunctionSymbol Print = new("print",
            ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);

        public static readonly FunctionSymbol Scan =
            new("scan", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);

        public static readonly FunctionSymbol Rand = new FunctionSymbol("rand",
            ImmutableArray.Create(new ParameterSymbol("max", TypeSymbol.Int)), TypeSymbol.Int);

        public static IEnumerable<FunctionSymbol> GetAll() =>
            typeof(BuiltInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(FunctionSymbol))
                .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}