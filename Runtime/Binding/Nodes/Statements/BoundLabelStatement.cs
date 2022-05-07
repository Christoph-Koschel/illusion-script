namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    public sealed class BoundLabelStatement : BoundStatement
    {
        public readonly BoundLabel BoundLabel;

        public BoundLabelStatement(BoundLabel boundLabel)
        {
            this.BoundLabel = boundLabel;
        }


        public override BoundNodeType boundType => BoundNodeType.LabelStatement;
    }
}