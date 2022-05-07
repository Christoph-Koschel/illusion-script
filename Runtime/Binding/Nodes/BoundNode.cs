using System.IO;

namespace IllusionScript.Runtime.Binding.Nodes
{
    public abstract class BoundNode
    {
        public abstract BoundNodeType boundType { get; }

        public override string ToString()
        {
            using StringWriter writer = new StringWriter();
            this.WriteTo(writer);
            return writer.ToString();
        }
    }
}