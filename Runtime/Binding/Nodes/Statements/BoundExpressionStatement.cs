using IllusionScript.Runtime.Binding.Nodes.Expressions;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public readonly BoundExpression expression;

        public BoundExpressionStatement(BoundExpression expression)
        {
            this.expression = expression;
        }

        public override BoundNodeType boundType => BoundNodeType.ExpressionStatement;
    }
}