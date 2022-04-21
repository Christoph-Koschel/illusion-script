using IllusionScript.Runtime.Parsing.Nodes.Expressions;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class ExpressionStatement : Statement
    {
        public readonly Expression expression;

        public ExpressionStatement(Expression expression)
        {
            this.expression = expression;
        }
        
        public override SyntaxType type => SyntaxType.ExpressionStatement;
    }
}