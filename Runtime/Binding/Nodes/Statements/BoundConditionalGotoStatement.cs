using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public readonly LabelSymbol label;
        public readonly BoundExpression condition;
        public readonly bool jmpIFalse;

        public BoundConditionalGotoStatement(LabelSymbol label, BoundExpression condition, bool jmpIFalse = false)
        {
            this.label = label;
            this.condition = condition;
            this.jmpIFalse = jmpIFalse;
        }

        public override BoundNodeType boundType => BoundNodeType.ConditionalGotoStatement;
    }
}