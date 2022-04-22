using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public readonly LabelSymbol label;

        public BoundGotoStatement(LabelSymbol label)
        {
            this.label = label;
        }

        public override BoundNodeType boundType => BoundNodeType.GotoStatement;
    }
}