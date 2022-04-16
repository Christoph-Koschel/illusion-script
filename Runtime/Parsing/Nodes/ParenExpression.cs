using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class ParenExpression : Expression
    {
        private readonly Token left;
        public readonly Expression expression;
        private readonly Token right;

        public ParenExpression(Token left, Expression expression, Token right)
        {
            this.left = left;
            this.expression = expression;
            this.right = right;
        }

        public override SyntaxType type => SyntaxType.ParenExpression;
    }
}