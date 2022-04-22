using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class IfStatement : Statement
    {
        public readonly Token keyword;
        public readonly Expression condition;
        public readonly Statement body;
        public readonly ElseClause elseClause;

        public IfStatement(Token keyword, Expression condition, Statement body, ElseClause elseClause)
        {
            this.keyword = keyword;
            this.condition = condition;
            this.body = body;
            this.elseClause = elseClause;
        }

        public override SyntaxType type => SyntaxType.IfStatement;
    }
}