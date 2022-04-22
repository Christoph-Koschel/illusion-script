using IllusionScript.Runtime.Binding.Nodes.Expressions;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public readonly BoundExpression condition;
        public readonly BoundStatement body;

        public BoundWhileStatement(BoundExpression condition, BoundStatement body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override BoundNodeType boundType => BoundNodeType.WhileStatement;
    }
}