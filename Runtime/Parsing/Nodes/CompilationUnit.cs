using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class CompilationUnit : Node
    {
        public readonly Expression expression;
        public readonly Token endOfFileToken;

        public CompilationUnit(Expression expression, Token endOfFileToken)
        {
            this.expression = expression;
            this.endOfFileToken = endOfFileToken;
        }

        public override SyntaxType type => SyntaxType.CompilationUnit;
    }
}