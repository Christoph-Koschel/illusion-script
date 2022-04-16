using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class LiteralExpression : Expression
    {
        public readonly Token token;
        public readonly object value;

        public LiteralExpression(Token token, object value)
        {
            this.token = token;
            this.value = value;
        }
        
        public override SyntaxType type => SyntaxType.LiteralExpression;
    }
}