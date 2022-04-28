using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class ReturnStatement : Statement
    {
        public readonly Token returnKeyword;
        public readonly Expression expression;

        public ReturnStatement(Token returnKeyword, Expression expression)
        {
            this.returnKeyword = returnKeyword;
            this.expression = expression;
        }


        public override SyntaxType type => SyntaxType.ReturnStatement;
        public override SyntaxType endToken => SyntaxType.SemicolonToken;
    }
}