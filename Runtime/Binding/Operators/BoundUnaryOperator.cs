using System;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Symbols;

namespace IllusionScript.Runtime.Binding.Operators
{
    internal sealed class BoundUnaryOperator
    {
        public readonly SyntaxType type;
        public readonly BoundUnaryOperatorType operatorType;
        public readonly TypeSymbol rightType;
        public readonly TypeSymbol resultType;

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

        private static readonly BoundUnaryOperator[] operators =
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
    }
}