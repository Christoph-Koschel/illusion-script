namespace IllusionScript.Runtime.Binding.Nodes
{
    public abstract class BoundNode
    {
        public readonly Parsing.Nodes.Node node;

        protected BoundNode(Parsing.Nodes.Node node)
        {
            this.node = node;
        }

        public abstract BoundNodeType boundType { get; }
    }
}