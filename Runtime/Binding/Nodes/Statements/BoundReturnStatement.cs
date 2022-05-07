using IllusionScript.Runtime.Binding.Nodes.Expressions;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    public sealed class BoundReturnStatement : BoundStatement
    {
        public readonly BoundExpression expression;

        public BoundReturnStatement(BoundExpression expression)
        {
            this.expression = expression;
        }

        public override BoundNodeType boundType => BoundNodeType.ReturnStatement;
    }
}