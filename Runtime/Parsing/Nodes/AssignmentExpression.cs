using System.Collections.Generic;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public sealed class AssignmentExpression : Expression
    {
        public override SyntaxType type => SyntaxType.AssignmentExpression;
        public readonly Token identifier;
        public readonly Node equalsToken;
        public readonly Expression expression;
        
        public AssignmentExpression(Token identifier, Node equalsToken, Expression expression)
        {
            this.identifier = identifier;
            this.equalsToken = equalsToken;
            this.expression = expression;
        }
    }
}