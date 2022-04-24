using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;
using IllusionScript.Runtime.Parsing.Nodes.Statements;

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
            Statement statement = ParseStatement();
            Token end = Match(SyntaxType.EOFToken);
            return new CompilationUnit(statement, end);
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

        private Statement ParseStatement()
        {
            return current.type switch
            {
                SyntaxType.LBraceToken => ParseBlockStatement(),
                SyntaxType.LetKeyword or SyntaxType.ConstKeyword => ParseVariableDeclaration(),
                SyntaxType.IfKeyword => ParseIfStatement(),
                SyntaxType.WhileKeyword => ParseWhileStatement(),
                SyntaxType.ForKeyword => ParseForStatement(),
                _ => ParseExpressionStatement()
            };
        }

        private Statement ParseForStatement()
        {
            Token keyword = Match(SyntaxType.ForKeyword);
            Token lParen = Match(SyntaxType.LParenToken);
            Token identifier = Match(SyntaxType.IdentifierToken);
            Token equalsToken = Match(SyntaxType.EqualsToken);
            Expression startExpression = ParseExpression();
            Token toKeyword = Match(SyntaxType.ToKeyword);
            Expression endExpression = ParseExpression();
            Token rParen = Match(SyntaxType.RParenToken);
            Statement body = ParseStatement();

            return new ForStatement(keyword, lParen, identifier, equalsToken, startExpression, toKeyword, endExpression,
                rParen, body);
        }

        private Statement ParseWhileStatement()
        {
            Token keyword = Match(SyntaxType.WhileKeyword);
            Token lParen = Match(SyntaxType.LParenToken);
            Expression condition = ParseExpression();
            Token rParen = Match(SyntaxType.RParenToken);
            Statement body = ParseStatement();

            return new WhileStatement(keyword, lParen, condition, rParen, body);
        }

        private Statement ParseIfStatement()
        {
            Token keyword = Match(SyntaxType.IfKeyword);
            Token lParen = Match(SyntaxType.LParenToken);
            Expression condition = ParseExpression();
            Token rParen = Match(SyntaxType.RParenToken);
            Statement statement = ParseStatement();
            ElseClause elseClause = ParseElseClause();

            return new IfStatement(keyword, lParen, condition, rParen, statement, elseClause);
        }

        private ElseClause ParseElseClause()
        {
            if (current.type != SyntaxType.ElseKeyword)
            {
                return null;
            }

            Token keyword = NextToken();
            Statement statement = ParseStatement();
            return new ElseClause(keyword, statement);
        }

        private Statement ParseVariableDeclaration()
        {
            SyntaxType expected =
                current.type == SyntaxType.LetKeyword ? SyntaxType.LetKeyword : SyntaxType.ConstKeyword;
            Token keyword = Match(expected);
            Token identifier = Match(SyntaxType.IdentifierToken);
            Token equals = Match(SyntaxType.EqualsToken);
            Expression initializer = ParseExpression();
            return new VariableDeclarationStatement(keyword, identifier, equals, initializer);
        }

        private Statement ParseBlockStatement()
        {
            ImmutableArray<Statement>.Builder statements = ImmutableArray.CreateBuilder<Statement>();

            Token lBrace = Match(SyntaxType.LBraceToken);
            while (current.type != SyntaxType.EOFToken && current.type != SyntaxType.RBraceToken)
            {
                Token startToken = current;

                Statement statement = ParseStatement();
                statements.Add(statement);

                if (current == startToken)
                {
                    NextToken();
                }
            }

            Token rBrace = Match(SyntaxType.RBraceToken);

            return new BlockStatement(lBrace, statements.ToImmutable(), rBrace);
        }

        private Statement ParseExpressionStatement()
        {
            Expression expression = ParseExpression();
            return new ExpressionStatement(expression);
        }

        private Expression ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private Expression ParseAssignmentExpression()
        {
            if (current.type == SyntaxType.IdentifierToken)
            {
                switch (Peek(1).type)
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
                        Token identifierToken = NextToken();
                        Token operatorToken = NextToken();
                        Expression right = ParseAssignmentExpression();
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
            else
            {
                return ParseNameExpression();
            }
        }

        private Expression ParseCallExpression()
        {
            Token identifier = Match(SyntaxType.IdentifierToken);
            Token lParen = Match(SyntaxType.LParenToken);
            SeparatedSyntaxList<Expression> arguments = ParseArguments();
            Token rParen = Match(SyntaxType.RParenToken);
            return new CallExpression(identifier, lParen, arguments, rParen);
        }

        private SeparatedSyntaxList<Expression> ParseArguments()
        {
            ImmutableArray<Node>.Builder nodesAndSeparators = ImmutableArray.CreateBuilder<Node>();

            while (current.type != SyntaxType.RParenToken &&
                   current.type != SyntaxType.EOFToken)
            {
                Expression expression = ParseExpression();
                nodesAndSeparators.Add(expression);

                if (current.type != SyntaxType.RParenToken)
                {
                    Token comma = Match(SyntaxType.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
            }
            
            return new SeparatedSyntaxList<Expression>(nodesAndSeparators.ToImmutable());
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

        private Expression ParseStringLiteral()
        {
            Token stringToken = Match(SyntaxType.StringToken);
            return new LiteralExpression(stringToken, stringToken.value);
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