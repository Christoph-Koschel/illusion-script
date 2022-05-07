using System;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Binding.Operators
{
    public sealed class BoundUnaryOperator
    {
        public SyntaxType type;
        public BoundUnaryOperatorType operatorType;
        public TypeSymbol rightType;
        public TypeSymbol resultType;

        private BoundUnaryOperator(SyntaxType type, BoundUnaryOperatorType operatorType, TypeSymbol rightType,
            TypeSymbol resultType)
        {
            this.type = type;
            this.operatorType = operatorType;
            this.rightType = rightType;
            this.resultType = resultType;
        }

        private BoundUnaryOperator(SyntaxType type, BoundUnaryOperatorType operatorType, TypeSymbol rightType)
            : this(type, operatorType, rightType, rightType)
        {
        }

        private static BoundUnaryOperator[] operators =
        {
            new(SyntaxType.TildeToken, BoundUnaryOperatorType.OnesComplement, TypeSymbol.Int),
            new(SyntaxType.BangToken, BoundUnaryOperatorType.LogicalNegation, TypeSymbol.Bool),
            new(SyntaxType.PlusToken, BoundUnaryOperatorType.Identity, TypeSymbol.Int),
            new(SyntaxType.MinusToken, BoundUnaryOperatorType.Negation, TypeSymbol.Int)
        };

        public static BoundUnaryOperator Bind(SyntaxType type, TypeSymbol rightType)
        {
            foreach (BoundUnaryOperator unaryOperator in operators)
            {
                if (unaryOperator.type == type && unaryOperator.rightType == rightType)
                {
                    return unaryOperator;
                }
            }

            return null;
        }

        public static string GetText(BoundUnaryOperatorType type)
        {
            return type switch
            {
                BoundUnaryOperatorType.Identity => "+",
                BoundUnaryOperatorType.Negation => "-",
                BoundUnaryOperatorType.LogicalNegation => "!",
                BoundUnaryOperatorType.OnesComplement => "~",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}