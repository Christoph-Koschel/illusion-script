using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public readonly BoundLabel BoundLabel;
        public readonly BoundExpression condition;
        public readonly bool JmpIfTrue;

        public BoundConditionalGotoStatement(BoundLabel boundLabel, BoundExpression condition, bool jmpIfTrue = true)
        {
            this.BoundLabel = boundLabel;
            this.condition = condition;
            this.JmpIfTrue = jmpIfTrue;
        }

        public override BoundNodeType boundType => BoundNodeType.ConditionalGotoStatement;
    }
}