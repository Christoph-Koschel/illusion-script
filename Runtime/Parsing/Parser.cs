using System.Collections.Generic;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Parsing
{
    public sealed class Parser : SyntaxFacts
    {
        private readonly SourceText text;
        private readonly Token[] tokens;
        private readonly DiagnosticGroup diagnostics;
        private int position;
        private Token current => Peek(0);

        internal Parser(SourceText text)
        {
            this.text = text;
            diagnostics = new DiagnosticGroup();
            position = 0;

            Lexer lexer = new Lexer(text);
            List<Token> tokens = new List<Token>();
            Token token;
            do
            {
                token = lexer.Lex();

                if (token.type != SyntaxType.WhiteSpaceToken && token.type != SyntaxType.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.type != SyntaxType.EOFToken);

            this.tokens = tokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics());
        }

        internal DiagnosticGroup Diagnostics => diagnostics;

        public CompilationUnit ParseCompilationUnit()
        {
            Expression expression = ParseExpression();
            Token end = Match(SyntaxType.EOFToken);
            return new CompilationUnit(expression, end);
        }

        private Token NextToken()
        {
            Token token = current;
            position++;
            return token;
        }

        private Token Peek(int offset)
        {
            int index = position + offset;
            return index < tokens.Length ? tokens[index] : tokens[^1];
        }

        private Token Match(SyntaxType type)
        {
            if (current.type == type)
            {
                return NextToken();
            }

            diagnostics.ReportUnexpectedToken(current.span, current.type, type);
            return new Token(type, current.position, null, null);
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
                        var identifierToken = NextToken();
                        var operatorToken = NextToken();
                        var right = ParseAssignmentExpression();
                        return new AssignmentExpression(identifierToken, operatorToken, right);
                }
            }

            return ParseBinaryExpression();
        }


        private Expression ParseBinaryExpression(int parentPrecedence = 0)
        {
            Expression left;
            int unaryIndex = GetUnaryOperatorPrecedence(current.type);
            if (unaryIndex != 0 && unaryIndex >= parentPrecedence)
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
                int index = GetBinaryOperatorPrecedence(current.type);
                if (index == 0 || index <= parentPrecedence)
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
                case SyntaxType.IdentifierToken:
                    return ParseNameExpression();
                case SyntaxType.NumberToken:
                default:
                    return ParseNumberLiteral();
            }
        }

        private Expression ParseNameExpression()
        {
            Token identifier = Match(SyntaxType.IdentifierToken);
            return new NameExpression(identifier);
        }

        private Expression ParseNumberLiteral()
        {
            Token numberToken = Match(SyntaxType.NumberToken);
            return new LiteralExpression(numberToken, numberToken.value);
        }

        private Expression ParseBooleanLiteral()
        {
            bool isTrue = current.type == SyntaxType.TrueKeyword;
            Token token = isTrue ? Match(SyntaxType.TrueKeyword) : Match(SyntaxType.FalseKeyword);
            return new LiteralExpression(token, isTrue);
        }

        private Expression ParseParenExpression()
        {
            Token left = Match(SyntaxType.LParenToken);
            Expression expression = ParseExpression();
            Token right = Match(SyntaxType.RParenToken);
            return new ParenExpression(left, expression, right);
        }
    }
}