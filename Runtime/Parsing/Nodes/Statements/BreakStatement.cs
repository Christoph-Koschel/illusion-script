using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public class BreakStatement : Statement
    {
        public readonly Token keyword;

        public BreakStatement(Token keyword)
        {
            this.keyword = keyword;
        }

        public override SyntaxType type => SyntaxType.BreakStatement;
    }
}