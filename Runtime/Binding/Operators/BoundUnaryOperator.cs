using System;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Binding.Operators
{
    internal sealed class BoundUnaryOperator
    {
        public SyntaxType type;
        public BoundUnaryOperatorType operatorType;
        public Type rightType;
        public Type resultType;
        
        private BoundUnaryOperator(SyntaxType type, BoundUnaryOperatorType operatorType,Type rightType, Type resultType)
        {
            this.type = type;
            this.operatorType = operatorType;
            this.rightType = rightType;
            this.resultType = resultType;
        }
        
        private BoundUnaryOperator(SyntaxType type, BoundUnaryOperatorType operatorType,Type rightType) 
            : this(type, operatorType, rightType, rightType)
        {
        }

        private static BoundUnaryOperator[] operators =
        {
            new (SyntaxType.TildeToken, BoundUnaryOperatorType.OnesComplement, typeof(int)),
            new (SyntaxType.BangToken, BoundUnaryOperatorType.LogicalNegation, typeof(bool)),
            new (SyntaxType.PlusToken, BoundUnaryOperatorType.Identity, typeof(int)),
            new (SyntaxType.MinusToken, BoundUnaryOperatorType.Negation, typeof(int))
        };

        public static BoundUnaryOperator Bind(SyntaxType type, Type rightType)
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