using System;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Binding.Operators
{
    public sealed class BoundBinaryOperator
    {
        public readonly SyntaxType type;
        public readonly BoundBinaryOperatorType operatorType;
        public readonly TypeSymbol leftType;
        public readonly TypeSymbol rightType;
        public readonly TypeSymbol resultType;

        private BoundBinaryOperator(SyntaxType type, BoundBinaryOperatorType operatorType, TypeSymbol leftType,
            TypeSymbol rightType, TypeSymbol resultType)
        {
            this.type = type;
            this.operatorType = operatorType;
            this.leftType = leftType;
            this.rightType = rightType;
            this.resultType = resultType;
        }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, TypeSymbol type)
            : this(syntaxType, operatorType, type, type, type)
        {
        }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, TypeSymbol type, TypeSymbol result)
            : this(syntaxType, operatorType, type, type, result)
        {
        }

        private static BoundBinaryOperator[] operators =
        {
            new(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, TypeSymbol.Int),
            new(SyntaxType.MinusToken, BoundBinaryOperatorType.Subtraction, TypeSymbol.Int),
            new(SyntaxType.StarToken, BoundBinaryOperatorType.Multiplication, TypeSymbol.Int),
            new(SyntaxType.SlashToken, BoundBinaryOperatorType.Division, TypeSymbol.Int),
            new(SyntaxType.PercentToken, BoundBinaryOperatorType.Modulo, TypeSymbol.Int),
            new(SyntaxType.DoubleStarToken, BoundBinaryOperatorType.Pow, TypeSymbol.Int),

            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.BangToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Bool),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Bool),

            new(SyntaxType.AndToken, BoundBinaryOperatorType.LogicalAnd, TypeSymbol.Bool),
            new(SyntaxType.SplitToken, BoundBinaryOperatorType.LogicalOr, TypeSymbol.Bool),

            new(SyntaxType.AndToken, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Int),
            new(SyntaxType.SplitToken, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Int),
            new(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Int),
            new(SyntaxType.DoubleLessToken, BoundBinaryOperatorType.BitwiseShiftLeft, TypeSymbol.Int),
            new(SyntaxType.DoubleGreaterToken, BoundBinaryOperatorType.BitwiseShiftRight, TypeSymbol.Int),

            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.LessToken, BoundBinaryOperatorType.Less, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.LessEqualsToken, BoundBinaryOperatorType.LessEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.GreaterToken, BoundBinaryOperatorType.Greater, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxType.GreaterEqualsToken, BoundBinaryOperatorType.GreaterEquals, TypeSymbol.Int,
                TypeSymbol.Bool),

            new(SyntaxType.AndToken, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Bool),
            new(SyntaxType.DoubleAndToken, BoundBinaryOperatorType.LogicalAnd,
                TypeSymbol.Bool),

            new(SyntaxType.SplitToken, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Bool),
            new(SyntaxType.DoubleSplitToken, BoundBinaryOperatorType.LogicalOr, TypeSymbol.Bool),
            new(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Bool),
            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Bool),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Bool),

            new(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, TypeSymbol.String),
            new(SyntaxType.DoubleEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.String,
                TypeSymbol.Bool),
            new(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.String,
                TypeSymbol.Bool),
        };

        public static BoundBinaryOperator Bind(SyntaxType type, TypeSymbol leftType, TypeSymbol rightType)
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

        public static string GetText(BoundBinaryOperatorType type)
        {
            return type switch
            {
                BoundBinaryOperatorType.Addition => "+",
                BoundBinaryOperatorType.Subtraction => "-",
                BoundBinaryOperatorType.Multiplication => "*",
                BoundBinaryOperatorType.Division => "/",
                BoundBinaryOperatorType.Modulo => "%",
                BoundBinaryOperatorType.Pow => "**",
                BoundBinaryOperatorType.LogicalAnd => "&&",
                BoundBinaryOperatorType.LogicalOr => "||",
                BoundBinaryOperatorType.NotEquals => "!=",
                BoundBinaryOperatorType.Equals => "==",
                BoundBinaryOperatorType.BitwiseAnd => "&",
                BoundBinaryOperatorType.BitwiseOr => "|",
                BoundBinaryOperatorType.BitwiseXor => "^",
                BoundBinaryOperatorType.BitwiseShiftLeft => "<<",
                BoundBinaryOperatorType.BitwiseShiftRight => ">>",
                BoundBinaryOperatorType.Less => "<",
                BoundBinaryOperatorType.LessEquals => "<=",
                BoundBinaryOperatorType.Greater => ">",
                BoundBinaryOperatorType.GreaterEquals => ">=",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}