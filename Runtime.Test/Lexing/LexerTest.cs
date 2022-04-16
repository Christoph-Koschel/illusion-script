using System;
using System.Collections.Generic;
using System.Linq;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing;
using Xunit;
using Xunit.Abstractions;

namespace Runtime.Test.Lexing
{
    public class LexerTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public LexerTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [MemberData(nameof(GetTokensWithWhitespacesData))]
        public void CheckLexerTokenInterpreter(string text, SyntaxType type)
        {
            Token[] tokens = SyntaxThree.MakeTokens(text).ToArray();
            Assert.Single(tokens);
            Assert.Equal(type, tokens[0].type);
            Assert.Equal(text, tokens[0].text);
        }

        [Theory]
        [MemberData(nameof(GetTokensPairData))]
        public static void CheckLexerTokenPairInterpreter(string text1, SyntaxType type1, string text2,
            SyntaxType type2)
        {
            string text = text1 + text2;
            Token[] tokens = SyntaxThree.MakeTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);
            Assert.Equal(type1, tokens[0].type);
            Assert.Equal(text1, tokens[0].text);

            Assert.Equal(type2, tokens[1].type);
            Assert.Equal(text2, tokens[1].text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach ((string text, SyntaxType type) token in GetTokens())
            {
                yield return new object[] { token.text, token.type };
            }
        }

        public static IEnumerable<object[]> GetTokensWithWhitespacesData()
        {
            foreach ((string text, SyntaxType type) token in GetTokens().Concat(GetWhitespaceTokens()))
            {
                yield return new object[] { token.text, token.type };
            }
        }

        public static IEnumerable<object[]> GetTokensPairData()
        {
            foreach ((string t1Text, SyntaxType t1Type, string t2Text, SyntaxType t2Type) pair in GetTokenPairs())
            {
                yield return new object[] { pair.t1Text, pair.t1Type, pair.t2Text, pair.t2Type };
            }
        }

        private static IEnumerable<(string text, SyntaxType type)> GetTokens()
        {
            IEnumerable<(string text, SyntaxType type)> fixedTokens = Enum.GetValues(typeof(SyntaxType))
                .Cast<SyntaxType>()
                .Select(k => (text: Lexer.GetText(k), k))
                .Where(t => t.text != null);

            IEnumerable<(string text, SyntaxType type)> dynamicTokens = new[]
            {
                ("1", SyntaxType.NumberToken),
                ("123", SyntaxType.NumberToken),
                ("a", SyntaxType.IdentifierToken),
                ("abc", SyntaxType.IdentifierToken),
                ("\"Test\"", SyntaxType.StringToken),
                ("\"Te\\\"st\\\"\"", SyntaxType.StringToken)
            };

            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(string text, SyntaxType type)> GetWhitespaceTokens()
        {
            return new[]
            {
                (" ", SyntaxType.WhiteSpaceToken),
                ("\n", SyntaxType.WhiteSpaceToken),
                ("\r", SyntaxType.WhiteSpaceToken),
                ("\t", SyntaxType.WhiteSpaceToken),
                ("  ", SyntaxType.WhiteSpaceToken)
            };
        }

        private static IEnumerable<(string t1Text, SyntaxType t1Type, string t2Text, SyntaxType t2Type)> GetTokenPairs()
        {
            foreach ((string text, SyntaxType type) t1 in GetTokens())
            {
                foreach ((string text, SyntaxType type) t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.type, t2.type))
                        yield return (t1.text, t1.type, t2.text, t2.type);
                }
            }
        }

        private static bool RequiresSeparator(SyntaxType t1Type, SyntaxType t2Type)
        {
            bool t1IsKeyword = t1Type.ToString().EndsWith("Keyword");
            bool t2IsKeyword = t2Type.ToString().EndsWith("Keyword");

            if (t1IsKeyword && t2IsKeyword)
            {
                return true;
            }

            if (t1IsKeyword && t2Type == SyntaxType.IdentifierToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.IdentifierToken && t2IsKeyword)
            {
                return true;
            }

            if (t1Type == SyntaxType.IdentifierToken && t2Type == SyntaxType.IdentifierToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.NumberToken && t2Type == SyntaxType.NumberToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.StarToken && t2Type == SyntaxType.StarToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.StarToken && t2Type == SyntaxType.DoubleStarToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.StarToken && t2Type == SyntaxType.StarEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.BangToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.BangToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.PercentToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.PercentToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.AndToken && t2Type == SyntaxType.AndToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.AndToken && t2Type == SyntaxType.DoubleAndToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.AndToken && t2Type == SyntaxType.AndEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.AndToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.AndToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.StarToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.StarToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.PlusToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.PlusToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.MinusToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.MinusToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.SlashToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.SlashToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.LessToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.LessToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.EqualsToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.EqualsToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.GreaterToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.GreaterToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.HatToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.HatToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.SplitToken && t2Type == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (t1Type == SyntaxType.SplitToken && t2Type == SyntaxType.DoubleEqualsToken)
            {
                return true;
            }


            if (t1Type == SyntaxType.SplitToken && t2Type == SyntaxType.SplitToken)
            {
                return true;
            }


            if (t1Type == SyntaxType.SplitToken && t2Type == SyntaxType.SplitEqualsToken)
            {
                return true;
            }


            if (t1Type == SyntaxType.SplitToken && t2Type == SyntaxType.DoubleSplitToken)
            {
                return true;
            }


            return false;
        }
    }
}