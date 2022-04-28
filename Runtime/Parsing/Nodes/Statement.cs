namespace IllusionScript.Runtime.Parsing.Nodes
{
    public abstract class Statement : Node
    {
        public abstract SyntaxType endToken { get; }
    }
}