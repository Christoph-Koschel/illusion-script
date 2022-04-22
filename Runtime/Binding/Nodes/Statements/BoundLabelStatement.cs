using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public readonly LabelSymbol label;

        public BoundLabelStatement(LabelSymbol label)
        {
            this.label = label;
        }


        public override BoundNodeType boundType => BoundNodeType.LabelStatement;
    }
}