using IllusionScript.Runtime.Binding.Nodes.Expressions;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundDoWhileStatement : BoundStatement
    {
        public readonly BoundStatement body;
        public readonly BoundExpression condition;

        public BoundDoWhileStatement(BoundStatement body, BoundExpression condition)
        {
            this.body = body;
            this.condition = condition;
        }

        public override BoundNodeType boundType => BoundNodeType.DoWhileStatement;
    }
}