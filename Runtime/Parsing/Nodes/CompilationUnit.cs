using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes.Statements;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class CompilationUnit : Node
    {
        public readonly Statement statement;
        public readonly Token endOfFileToken;

        public CompilationUnit(Statement statement, Token endOfFileToken)
        {
            this.statement = statement;
            this.endOfFileToken = endOfFileToken;
        }

        public override SyntaxType type => SyntaxType.CompilationUnit;
    }
}