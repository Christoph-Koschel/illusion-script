using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion NONE = new (false, false, false);
        public static readonly Conversion IDENTITY = new (true, true, true);
        public static readonly Conversion IMPLICIT = new (true, false, true);
        public static readonly Conversion EXPLICIT = new (true, false, false);

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
            {
                return IDENTITY;
            }

            if (from != TypeSymbol.Void && to == TypeSymbol.Any)
            {
                return IMPLICIT;
            }

            if (from == TypeSymbol.Any && to != TypeSymbol.Void)
            {
                return EXPLICIT;
            }

            if (from == TypeSymbol.Bool || from == TypeSymbol.Int)
            {
                if (to == TypeSymbol.String)
                {
                    return EXPLICIT;
                }
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Bool || to == TypeSymbol.Int)
                {
                    return EXPLICIT;
                }
            }

            return NONE;
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