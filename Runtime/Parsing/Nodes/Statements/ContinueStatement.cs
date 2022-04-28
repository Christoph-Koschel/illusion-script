using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public class ContinueStatement : Statement
    {
        public readonly Token keyword;

        public ContinueStatement(Token keyword)
        {
            this.keyword = keyword;
        }
        
        public override SyntaxType type => SyntaxType.ContinueStatement;
    }
}