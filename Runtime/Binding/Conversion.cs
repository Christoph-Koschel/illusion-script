using System.Data;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new(false, false, false);
        public static readonly Conversion Identity = new(true, true, true);
        public static readonly Conversion Implicit = new(true, false, true);
        public static readonly Conversion Explicit = new Conversion(true, false, false);

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
            {
                return Identity;
            }

            if (from != TypeSymbol.Void && to == TypeSymbol.Object)
            {
                return Implicit;
            }

            if (from == TypeSymbol.Object && to == TypeSymbol.Void)
            {
                return Explicit;
            }

            if (from == TypeSymbol.Bool || from == TypeSymbol.Int)
            {
                if (to == TypeSymbol.String)
                {
                    return Explicit;
                }
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Bool || to == TypeSymbol.Int)
                {
                    return Explicit;
                }
            }

            return None;
        }

        public readonly bool exists;
        public readonly bool isIdentity;
        public readonly bool isImplicit;
        public bool isExplicit => exists && !isImplicit;

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            this.exists = exists;
            this.isIdentity = isIdentity;
            this.isImplicit = isImplicit;
        }
    }
}