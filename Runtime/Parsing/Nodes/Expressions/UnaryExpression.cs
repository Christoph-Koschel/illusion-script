using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Expressions
{
    public class UnaryExpression : Expressions.Expression
    {
        public override SyntaxType type => SyntaxType.UnaryExpression;
        public readonly Token operatorToken;
        public readonly Expressions.Expression right;
        
        public UnaryExpression(Token operatorToken, Expressions.Expression right)
        {
            this.operatorToken = operatorToken;
            this.right = right;
        }
    }
}