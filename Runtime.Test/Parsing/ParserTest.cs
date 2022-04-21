using System.Collections.Generic;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;
using Runtime.Test;
using Xunit;
using Xunit.Abstractions;

namespace IllusionScript.Runtime.Test.Parsing
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
            string? op1Text = SyntaxFacts.GetText(op1);
            string? op2Text = SyntaxFacts.GetText(op2);
            string text = $"1 {op1Text} 2 {op2Text} 3";
            Expression expression = SyntaxTree.Parse(text).root.expression; 

            testOutputHelper.WriteLine(SyntaxFacts.GetBinaryOperatorPrecedence(op1).ToString());
            testOutputHelper.WriteLine(SyntaxFacts.GetBinaryOperatorPrecedence(op2).ToString());
            testOutputHelper.WriteLine(text);
            testOutputHelper.WriteLine(expression.ToString());

            if (SyntaxFacts.GetBinaryOperatorPrecedence(op1) == SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxType.DoubleStarToken) ||
                SyntaxFacts.GetBinaryOperatorPrecedence(op2) == SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxType.DoubleStarToken))
            {
                if (SyntaxFacts.GetBinaryOperatorPrecedence(op1) < SyntaxFacts.GetBinaryOperatorPrecedence(op2))
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
                if (SyntaxFacts.GetBinaryOperatorPrecedence(op1) < SyntaxFacts.GetBinaryOperatorPrecedence(op2))
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
            var unaryText = SyntaxFacts.GetText(unaryType);
            var binaryText = SyntaxFacts.GetText(binaryType);
            string text = $"{unaryText} 1 {binaryText} 2";
            Expression expression = SyntaxTree.Parse(text).root.expression; 

            testOutputHelper.WriteLine(SyntaxFacts.GetUnaryOperatorPrecedence(unaryType).ToString());
            testOutputHelper.WriteLine(SyntaxFacts.GetBinaryOperatorPrecedence(binaryType).ToString());
            testOutputHelper.WriteLine(text);
            testOutputHelper.WriteLine(expression.ToString());

            if (SyntaxFacts.GetUnaryOperatorPrecedence(unaryType) < SyntaxFacts.GetBinaryOperatorPrecedence(binaryType))
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