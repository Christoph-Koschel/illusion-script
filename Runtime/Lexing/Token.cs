using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Lexing
{
    public sealed class Token : Expression
    {
        public readonly string text;
        public readonly object value;
        public readonly TextSpan span;

        public Token(SyntaxType type, int start, string text, object value)
        {
            this.type = type;
            this.text = text;
            this.value = value;

            span = new TextSpan(start, text.Length);
        }

        public override SyntaxType type { get; }
    }
}