using System.Text;
using IllusionScript.Runtime.Diagnostics;

namespace IllusionScript.Runtime.Lexing
{
    internal sealed class Lexer
    {
        private SourceText source;
        public readonly DiagnosticGroup diagnostics;
        private int position;
        private char current => Peek(0);
        private char next => Peek(1);

        public Lexer(SourceText source)
        {
            this.source = source;
            diagnostics = new DiagnosticGroup();
            position = 0;
        }

        private char Peek(int offset)
        {
            int index = position + offset;

            if (index >= source.text.Length)
            {
                return '\0';
            }

            return source.text[index];
        }

        public Token Lex()
        {
            int start = position;

            switch (current)
            {
                case '\0':
                    return new Token(SyntaxType.EOFToken, start, "", null);
                case '\r':
                case '\n':
                case ' ':
                    return CreateWhitespaces();
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return CreateNumber();
                case '+':
                    position++;
                    return new Token(SyntaxType.PlusToken, start, "+", null);
                case '-':
                    position++;
                    return new Token(SyntaxType.MinusToken, start, "-", null);
                case '*':
                    position++;
                    return new Token(SyntaxType.StarToken, start, "*", null);
                case '/':
                    position++;
                    return new Token(SyntaxType.SlashToken, start, "/", null);
                case '%':
                    position++;
                    return new Token(SyntaxType.PercentToken, start, "%", null);
                default:
                    diagnostics.ReportBadCharacter(start, current);
                    position++;
                    return new Token(SyntaxType.BadToken, start, current.ToString(), null);
            }
        }

        private Token CreateNumber()
        {
            StringBuilder builder = new StringBuilder();
            int start = position;

            while (char.IsDigit(current))
            {
                builder.Append(current);
                position++;
            }

            if (int.TryParse(builder.ToString(), out int number))
            {
                return new Token(SyntaxType.NumberToken, start, builder.ToString(), number);
            }
            else
            {
                diagnostics.ReportInvalidNumber(new TextSpan(start, builder.Length), builder.ToString(), typeof(int));
                return new Token(SyntaxType.BadToken, start, builder.ToString(), null);
            }
        }

        private Token CreateWhitespaces()
        {
            int start = position;
            StringBuilder builder = new StringBuilder();

            while (char.IsWhiteSpace(current))
            {
                builder.Append(current);
                position++;
            }

            return new Token(SyntaxType.WhiteSpaceToken, start, builder.ToString(), null);
        }
    }
}