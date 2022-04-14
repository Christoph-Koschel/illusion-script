using System;
using System.Collections.Generic;
using System.Linq;
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
        private Token next => Peek(1);
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

        private Expression ParseExpression()
        {
            return ParseBinaryOperator(ParseTerm, SyntaxType.StarToken, SyntaxType.SlashToken, SyntaxType.PercentToken);
        }

        private Expression ParseTerm()
        {
            return ParseBinaryOperator(ParseFactor, SyntaxType.PlusToken, SyntaxType.MinusToken);
        }

        private Expression ParseFactor()
        {
            Token token = current;

            if (token.type is SyntaxType.PlusToken or SyntaxType.MinusToken)
            {
                position++;
                Expression factor = ParseFactor();
                return new UnaryExpression(token, factor);
            }
            else if (token.type is SyntaxType.NumberToken)
            {
                position++;
                return new NumberExpression(token);
            }
            else if (token.type == SyntaxType.LParenToken)
            {
                position++;
                Expression expression = ParseExpression();
                if (current.type == SyntaxType.RParenToken)
                {
                    return new ParenExpression(expression);
                }
                else
                {
                    diagnostics.ReportExpectedToken(token.span, SyntaxType.RParenToken, token.type);
                }
            }

            diagnostics.ReportExpectedToken(token.span, SyntaxType.LParenToken, token.type);
            throw new Exception("Undefined operation");
        }

        private Expression ParseBinaryOperator(Func<Expression> func1, params SyntaxType[] operators)
        {
            return ParseBinaryOperator(func1, func1, operators);
        }

        private Expression ParseBinaryOperator(Func<Expression> func1, Func<Expression> func2,
            params SyntaxType[] operators)
        {
            Expression left = func1();

            while (operators.Contains(current.type))
            {
                Token operatorToken = current;
                position++;
                Expression right = func2();

                left = new BinaryOperationExpression(left, operatorToken, right);
            }

            return left;
        }

        public static int GetUnaryOperatorIndex(SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.MinusToken:
                case SyntaxType.PlusToken:
                    return 3;

                default: 
                    return 0;
            }
        }

        public static int GetBinaryOperatorIndex(SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.StarToken:
                case SyntaxType.SlashToken:
                case SyntaxType.PercentToken:
                    return 2;
                case SyntaxType.MinusToken:
                case SyntaxType.PlusToken:
                    return 1;
                default: 
                    return 0;
            }
        }
    }
}