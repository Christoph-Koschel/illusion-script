using System;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Binding.Operators
{
    internal sealed class BoundBinaryOperator
    {
        public SyntaxType type;
        public BoundBinaryOperatorType operatorType;
        public Type leftType;
        public Type rightType;
        public Type resultType;

        private BoundBinaryOperator(SyntaxType type, BoundBinaryOperatorType operatorType, Type leftType,
            Type rightType, Type resultType)
        {
            this.type = type;
            this.operatorType = operatorType;
            this.leftType = leftType;
            this.rightType = rightType;
            this.resultType = resultType;
        }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, Type type)
            : this(syntaxType, operatorType, type, type, type)
        {
        }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, Type type, Type result)
            : this(syntaxType, operatorType, type, type, result)
        {
        }

        private static BoundBinaryOperator[] operators =
        {
            new(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, typeof(int)),
            new(SyntaxType.MinusToken, BoundBinaryOperatorType.Subtraction, typeof(int)),
            new(SyntaxType.StarToken, BoundBinaryOperatorType.Multiplication, typeof(int)),
            new(SyntaxType.SlashToken, BoundBinaryOperatorType.Division, typeof(int)),
            new(SyntaxType.PercentToken, BoundBinaryOperatorType.Modulo, typeof(int)),
            new(SyntaxType.DoubleStarToken, BoundBinaryOperatorType.Pow, typeof(int)),

            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, typeof(int), typeof(bool)),
            new(SyntaxType.BangToken, BoundBinaryOperatorType.NotEquals, typeof(int), typeof(bool)),
            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, typeof(bool)),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(bool)),

            new(SyntaxType.AndToken, BoundBinaryOperatorType.LogicalAnd, typeof(bool)),
            new(SyntaxType.SplitToken, BoundBinaryOperatorType.LogicalOr, typeof(bool)),

            new(SyntaxType.AndToken, BoundBinaryOperatorType.BitwiseAnd, typeof(int)),
            new(SyntaxType.SplitToken, BoundBinaryOperatorType.BitwiseOr, typeof(int)),
            new(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, typeof(int)),
            new(SyntaxType.DoubleLessToken, BoundBinaryOperatorType.BitwiseShiftLeft, typeof(int)),
            new(SyntaxType.DoubleGreaterToken, BoundBinaryOperatorType.BitwiseShiftRight, typeof(int)),

            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, typeof(int), typeof(bool)),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(int), typeof(bool)),
            new(SyntaxType.LessToken, BoundBinaryOperatorType.Less, typeof(int), typeof(bool)),
            new(SyntaxType.LessEqualsToken, BoundBinaryOperatorType.LessEquals, typeof(int), typeof(bool)),
            new(SyntaxType.GreaterToken, BoundBinaryOperatorType.Greater, typeof(int), typeof(bool)),
            new(SyntaxType.GreaterEqualsToken, BoundBinaryOperatorType.GreaterEquals, typeof(int),
                typeof(bool)),

            new(SyntaxType.AndToken, BoundBinaryOperatorType.BitwiseAnd, typeof(bool)),
            new(SyntaxType.DoubleAndToken, BoundBinaryOperatorType.LogicalAnd,
                typeof(bool)),

            new(SyntaxType.SplitToken, BoundBinaryOperatorType.BitwiseOr, typeof(bool)),
            new(SyntaxType.DoubleSplitToken, BoundBinaryOperatorType.LogicalOr, typeof(bool)),
            new(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, typeof(bool)),
            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, typeof(bool)),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(bool)),

            new(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, typeof(string)),
            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, typeof(string),
                typeof(bool)),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(string),
                typeof(bool)),

            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, typeof(object)),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(object))
        };

        public static BoundBinaryOperator Bind(SyntaxType type, Type leftType, Type rightType)
        {
            foreach (BoundBinaryOperator binaryOperator in operators)
            {
                if (binaryOperator.type == type && binaryOperator.leftType == leftType &&
                    binaryOperator.rightType == rightType)
                {
                    return binaryOperator;
                }
            }

            return null;
        }
    }
}