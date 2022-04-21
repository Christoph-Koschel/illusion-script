using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Lexing
{
    public class Token : Node
    {
        public readonly int position;
        public override TextSpan span => new TextSpan(position, text?.Length ?? 0);
        public readonly string text;
        public readonly object value;

        public Token(SyntaxType type, int position, string text, object value)
        {
            this.type = type;
            this.position = position;
            this.text = text;
            this.value = value;
        }

        public override SyntaxType type { get; }
    }
}