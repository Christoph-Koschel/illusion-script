namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class ParenExpression : Expression
    {
        public readonly Expression expression;

        public ParenExpression(Expression expression)
        {
            this.expression = expression;
        }

        public override SyntaxType type => SyntaxType.ParenExpression;
    }
}