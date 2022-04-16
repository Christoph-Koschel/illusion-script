using System.Collections.Generic;
using System.Diagnostics;
using IllusionScript.Runtime.Interface;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;
using Xunit;
using Xunit.Abstractions;

namespace Runtime.Test.Parsing
{
    public class ParserTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ParserTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void ParserBinaryExpressionPrecedences(SyntaxType op1, SyntaxType op2)
        {
            string? op1Text = Lexer.GetText(op1);
            string? op2Text = Lexer.GetText(op2);
            string text = $"1 {op1Text} 2 {op2Text} 3";
            Expression expression = ParseExpression(text);

            testOutputHelper.WriteLine(Parser.GetBinaryOperatorIndex(op1).ToString());
            testOutputHelper.WriteLine(Parser.GetBinaryOperatorIndex(op2).ToString());
            testOutputHelper.WriteLine(text);
            testOutputHelper.WriteLine(expression.ToString());

            if (Parser.GetBinaryOperatorIndex(op1) == Parser.GetBinaryOperatorIndex(SyntaxType.DoubleStarToken) ||
                Parser.GetBinaryOperatorIndex(op2) == Parser.GetBinaryOperatorIndex(SyntaxType.DoubleStarToken))
            {
                if (Parser.GetBinaryOperatorIndex(op1) < Parser.GetBinaryOperatorIndex(op2))
                {
                    using AssertingEnumerator e = new AssertingEnumerator(expression);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "1");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "2");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "3");
                }
                else
                {
                    using AssertingEnumerator e = new AssertingEnumerator(expression);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "1");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "2");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "3");
                }
            }
            else
            {
                if (Parser.GetBinaryOperatorIndex(op1) < Parser.GetBinaryOperatorIndex(op2))
                {
                    using AssertingEnumerator e = new AssertingEnumerator(expression);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "1");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "2");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "3");
                }
                else
                {
                    using AssertingEnumerator e = new AssertingEnumerator(expression);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.BinaryExpression);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "1");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "2");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxType.LiteralExpression);
                    e.AssertToken(SyntaxType.NumberToken, "3");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void ParserUnaryExpressionPrecedences(SyntaxType unaryType, SyntaxType binaryType)
        {
            var unaryText = Lexer.GetText(unaryType);
            var binaryText = Lexer.GetText(binaryType);
            string text = $"{unaryText} 1 {binaryText} 2";
            Expression expression = ParseExpression(text);

            testOutputHelper.WriteLine(Parser.GetUnaryOperatorIndex(unaryType).ToString());
            testOutputHelper.WriteLine(Parser.GetBinaryOperatorIndex(binaryType).ToString());
            testOutputHelper.WriteLine(text);
            testOutputHelper.WriteLine(expression.ToString());

            if (Parser.GetUnaryOperatorIndex(unaryType) < Parser.GetBinaryOperatorIndex(binaryType))
            {
                using AssertingEnumerator e = new AssertingEnumerator(expression);
                e.AssertNode(SyntaxType.UnaryExpression);
                e.AssertToken(unaryType, unaryText);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.LiteralExpression);
                e.AssertToken(SyntaxType.NumberToken, "1");
                e.AssertToken(binaryType, binaryText);
                e.AssertNode(SyntaxType.LiteralExpression);
                e.AssertToken(SyntaxType.NumberToken, "2");
            }
            else
            {
                using AssertingEnumerator e = new AssertingEnumerator(expression);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.UnaryExpression);
                e.AssertToken(unaryType, unaryText);
                e.AssertNode(SyntaxType.LiteralExpression);
                e.AssertToken(SyntaxType.NumberToken, "1");
                e.AssertToken(binaryType, binaryText);
                e.AssertNode(SyntaxType.LiteralExpression);
                e.AssertToken(SyntaxType.NumberToken, "2");
            }
        }

        private static Expression ParseExpression(string text)
        {
            Compilation compilation = SyntaxThree.Parse(text);
            return compilation.syntaxThree.root;
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (var op1 in GetBinaryOperatorTypes())
            {
                foreach (var op2 in GetBinaryOperatorTypes())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (var unary in GetUnaryOperatorTypes())
            {
                foreach (var binary in GetBinaryOperatorTypes())
                {
                    yield return new object[] { unary, binary };
                }
            }
        }

        private static IEnumerable<SyntaxType> GetUnaryOperatorTypes()
        {
            yield return SyntaxType.PlusToken;
            yield return SyntaxType.MinusToken;
        }

        private static IEnumerable<SyntaxType> GetBinaryOperatorTypes()
        {
            yield return SyntaxType.PlusToken;
            yield return SyntaxType.MinusToken;
            yield return SyntaxType.StarToken;
            yield return SyntaxType.DoubleStarToken;
            yield return SyntaxType.SlashToken;
            yield return SyntaxType.PercentToken;
        }
    }
}