using System.Collections.Generic;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public sealed class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, Token operatorToken, Expression right)
        {
            this.left = left;
            this.operatorToken = operatorToken;
            this.right = right;
        }
        
        public override SyntaxType type => SyntaxType.BinaryExpression;
        public readonly Expression left;
        public readonly Token operatorToken;
        public readonly Expression right;
    }
}