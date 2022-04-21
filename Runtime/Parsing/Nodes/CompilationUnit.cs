using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class CompilationUnit : Node
    {
        public readonly Expression expression;
        public readonly Token endOfFile;

        public CompilationUnit(Expression expression, Token endOfFile)
        {
            this.expression = expression;
            this.endOfFile = endOfFile;
        }

        public override SyntaxType type => SyntaxType.CompilationUnit;
    }
}