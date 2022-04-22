using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Statements;

namespace IllusionScript.Runtime.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter

    {
        private Lowerer()
        {
        }

        public static BoundStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
        }
    }
}