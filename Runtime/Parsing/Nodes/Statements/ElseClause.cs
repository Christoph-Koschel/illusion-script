using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class ElseClause : Node
    {
        public readonly Token keyword;
        public readonly Statement body;

        public ElseClause(Token keyword, Statement body)
        {
            this.keyword = keyword;
            this.body = body;
        }

        public override SyntaxType type => SyntaxType.ElseClause;
    }
}