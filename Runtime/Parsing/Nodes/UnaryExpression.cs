using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class UnaryExpression : Expression
    {
        public readonly Token operatorToken;
        public readonly Expression factor;

        public UnaryExpression(Token operatorToken, Expression factor)
        {
            this.operatorToken = operatorToken;
            this.factor = factor;
        }
        
        public override SyntaxType type => SyntaxType.UnaryExpression;
    }
}