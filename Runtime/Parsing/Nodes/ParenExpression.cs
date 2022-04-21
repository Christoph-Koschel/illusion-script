using System.Collections.Generic;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class ParenExpression : Expression
    {
        public override SyntaxType type => SyntaxType.ParenExpression;
        public readonly Token openToken;
        public readonly Expression expression;
        public readonly Token closeToken;
        
        public ParenExpression(Token openToken, Expression expression, Token closeToken)
        {
            this.openToken = openToken;
            this.expression = expression;
            this.closeToken = closeToken;
        }


    }
}