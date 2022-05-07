namespace IllusionScript.Runtime.Binding.Nodes
{
    public sealed class BoundLabel
    {
        public readonly string name;

        public BoundLabel(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}