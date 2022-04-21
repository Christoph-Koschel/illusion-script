using System.Collections.Generic;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class UnaryExpression : Expression
    {
        public override SyntaxType type => SyntaxType.UnaryExpression;
        public readonly Token operatorToken;
        public readonly Expression right;
        
        public UnaryExpression(Token operatorToken, Expression right)
        {
            this.operatorToken = operatorToken;
            this.right = right;
        }
    }
}