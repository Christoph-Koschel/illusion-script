using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Expressions
{
    public class ParenExpression : Expressions.Expression
    {
        public override SyntaxType type => SyntaxType.ParenExpression;
        public readonly Token openToken;
        public readonly Expressions.Expression expression;
        public readonly Token closeToken;
        
        public ParenExpression(Token openToken, Expressions.Expression expression, Token closeToken)
        {
            this.openToken = openToken;
            this.expression = expression;
            this.closeToken = closeToken;
        }


    }
}