using IllusionScript.Runtime.Diagnostics;

namespace IllusionScript.Runtime.Lexing
{
    public sealed class Lexer
    {
        private TextSource source;
        public readonly DiagnosticGroup diagnostics;
        private int position;
        private char current => Peek(0);
        private char next => Peek(1);

        public Lexer(TextSource source)
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

            if (position > source.text.Length)
            {
                return new Token(SyntaxType.EOFToken, start, "", null);
            }

            switch (current)
            {
                default:
                    diagnostics.ReportBadCharacter(start, current);
                    position++;
                    break;
            }
        }
        
    }
}