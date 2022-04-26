using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Expressions
{
    public sealed class LiteralExpression : Expression
    {
        public override SyntaxType type => SyntaxType.LiteralExpression;
        public readonly Token literalToken;
        public readonly object value;

        public LiteralExpression(Token literalToken, object value)
        {
            this.literalToken = literalToken;
            this.value = value;
        }

        public LiteralExpression(Token literalToken) 
            : this(literalToken, literalToken.value)
        {
        }
    }
}