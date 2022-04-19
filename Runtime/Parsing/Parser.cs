using System;
using System.Collections.Generic;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Parsing
{
    public sealed class Parser
    {
        public readonly DiagnosticGroup diagnostics;
        public readonly Token[] tokens;
        private Token current => Peek(0);
        private int position;

        internal Parser(SourceText source)
        {
            Lexer lexer = new Lexer(source);
            List<Token> tokens = new List<Token>();
            do
            {
                Token token = lexer.Lex();
                if (token.type != SyntaxType.WhiteSpaceToken)
                {
                    tokens.Add(token);
                }
            } while (tokens[^1].type != SyntaxType.EOFToken);

            diagnostics = new DiagnosticGroup();
            diagnostics.AddRange(lexer.diagnostics);

            this.tokens = tokens.ToArray();
            position = 0;
        }

        public Expression ParseText()
        {
            return ParseExpression(); // TODO Change to statements
        }

        private Token Peek(int offset)
        {
            int index = position + offset;
            return index < tokens.Length ? tokens[index] : tokens[^1];
        }

        private Token NextToken()
        {
            Token current = this.current;
            position++;
            return current;
        }

        private Token MatchToken(SyntaxType type)
        {
            if (current.type == type)
            {
                return NextToken();
            }

            diagnostics.ReportUnexpectedToken(current.span, current.type, type);
            return new Token(type, current.span.start, null, null);
        }

        private Expression ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private Expression ParseAssignmentExpression()
        {
            if (current.type == SyntaxType.IdentifierToken)
            {
                switch (current.type)
                {
                    case SyntaxType.PlusEqualsToken:
                    case SyntaxType.MinusEqualsToken:
                    case SyntaxType.StarEqualsToken:
                    case SyntaxType.DoubleStarEqualsToken:
                    case SyntaxType.SlashEqualsToken:
                    case SyntaxType.AndEqualsToken:
                    case SyntaxType.SplitEqualsToken:
                    case SyntaxType.HatEqualsToken:
                    case SyntaxType.PercentEqualsToken:
                    case SyntaxType.EqualsToken:
                        var identifierToken = current;
                        position++;
                        var operatorToken = current;
                        position++;
                        var right = ParseAssignmentExpression();
                        return new AssignmentExpression(identifierToken, operatorToken, right);
                }
            }

            return ParseBinaryExpression();
        }

        private Expression ParseBinaryExpression(int parentIndex = 0)
        {
            Expression left;
            int unaryIndex = GetUnaryOperatorIndex(current.type);
            if (unaryIndex != 0 && unaryIndex >= parentIndex)
            {
                Token operatorToken = NextToken();
                Expression right = ParseBinaryExpression(unaryIndex);
                left = new UnaryExpression(operatorToken, right);
            }
            else
            {
                left = ParseBaseExpression();
            }

            while (true)
            {
                int index = GetBinaryOperatorIndex(current.type);
                if (index == 0 || index <= parentIndex)
                {
                    break;
                }

                Token operatorToken = NextToken();
                Expression right = ParseBinaryExpression(index);
                left = new BinaryExpression(left, operatorToken, right);
            }

            return left;
        }

        private Expression ParseBaseExpression()
        {
            switch (current.type)
            {
                case SyntaxType.LParenToken:
                    return ParseParenExpression();
                case SyntaxType.FalseKeyword:
                case SyntaxType.TrueKeyword:
                    return ParseBooleanLiteral();
                case SyntaxType.NumberToken:
                    return ParseNumberLiteral();
                case SyntaxType.StringToken:
                    return ParseStringLiteral();
                case SyntaxType.IdentifierToken:
                default:
                    return ParseNameOrCallExpression();
            }
        }

        private Expression ParseNameOrCallExpression()
        {
            if (Peek(0).type == SyntaxType.IdentifierToken && Peek(1).type == SyntaxType.LParenToken)
            {
                return ParseCallExpression();
            }

            return ParseNameExpression();
        }

        private Expression ParseNameExpression()
        {
            Token identifier = MatchToken(SyntaxType.IdentifierToken);
            return new NameExpression(identifier);
        }

        private Expression ParseCallExpression()
        {
            Token identifier = MatchToken(SyntaxType.IdentifierToken);
            Token lParen = MatchToken(SyntaxType.LParenToken);
            Node[] arguments = ParseArguments();
            Token rParen = MatchToken(SyntaxType.RParenToken);
            return new CallExpression(identifier, lParen, arguments, rParen);
        }

        private Node[] ParseArguments()
        {
            List<Node> nodes = new List<Node>();

            bool parseNextArg = true;
            while (parseNextArg && current.type is not SyntaxType.RParenToken and not SyntaxType.EOFToken)
            {
                Expression expression = ParseExpression();
                nodes.Add(expression);

                if (current.type == SyntaxType.CommaToken)
                {
                    Token comma = MatchToken(SyntaxType.CommaToken);
                    nodes.Add(comma);
                }
                else
                {
                    parseNextArg = false;
                }
            }

            return nodes.ToArray();
        }

        private Expression ParseStringLiteral()
        {
            Token stringToken = MatchToken(SyntaxType.StringToken);
            return new LiteralExpression(stringToken, stringToken.value);
        }

        private Expression ParseNumberLiteral()
        {
            Token numberToken = MatchToken(SyntaxType.NumberToken);
            return new LiteralExpression(numberToken, numberToken.value);
        }

        private Expression ParseBooleanLiteral()
        {
            bool isTrue = current.type == SyntaxType.TrueKeyword;
            Token token = isTrue ? MatchToken(SyntaxType.TrueKeyword) : MatchToken(SyntaxType.FalseKeyword);
            return new LiteralExpression(token, isTrue);
        }

        private Expression ParseParenExpression()
        {
            Token left = MatchToken(SyntaxType.LParenToken);
            Expression expression = ParseExpression();
            Token right = MatchToken(SyntaxType.RParenToken);
            return new ParenExpression(left, expression, right);
        }

        public static int GetUnaryOperatorIndex(SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.MinusToken:
                case SyntaxType.PlusToken:
                case SyntaxType.BangToken:
                case SyntaxType.TildeToken:
                    return 6;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorIndex(SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.DoubleStarToken:
                    return 7;
                case SyntaxType.StarToken:
                case SyntaxType.SlashToken:
                case SyntaxType.PercentToken:
                    return 5;

                case SyntaxType.MinusToken:
                case SyntaxType.PlusToken:
                    return 4;
                case SyntaxType.DoubleEqualsToken:
                case SyntaxType.BangEqualsToken:
                case SyntaxType.LessToken:
                case SyntaxType.LessEqualsToken:
                case SyntaxType.GreaterToken:
                case SyntaxType.GreaterEqualsToken:
                    return 3;
                case SyntaxType.AndToken:
                case SyntaxType.DoubleAndToken:
                    return 2;
                case SyntaxType.SplitToken:
                case SyntaxType.DoubleSplitToken:
                case SyntaxType.HatToken:
                    return 1;
                default:
                    return 0;
            }
        }
        
        public static SyntaxType GetBinaryOperatorOfAssignmentOperator(SyntaxType type)
        {
            switch(type)
            {
                case SyntaxType.PlusEqualsToken:
                    return SyntaxType.PlusToken;
                case SyntaxType.MinusEqualsToken:
                    return SyntaxType.MinusToken;
                case SyntaxType.StarEqualsToken:
                    return SyntaxType.StarToken;
                case SyntaxType.SlashEqualsToken:
                    return SyntaxType.SlashToken;
                case SyntaxType.AndEqualsToken:
                    return SyntaxType.AndToken;
                case SyntaxType.SplitEqualsToken:
                    return SyntaxType.SplitToken;
                case SyntaxType.HatEqualsToken:
                    return SyntaxType.HatToken;
                default:
                    throw new Exception($"Unexpected syntax: '{type}'");
            }
        }
    }
}