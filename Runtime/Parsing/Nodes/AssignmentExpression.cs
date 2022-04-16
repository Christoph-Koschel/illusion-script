using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class AssignmentExpression : Expression
    {
        public readonly Token identifier;
        public readonly Token operatorToken;
        public readonly Expression right;

        public AssignmentExpression(Token identifier, Token operatorToken, Expression right)
        {
            this.identifier = identifier;
            this.operatorToken = operatorToken;
            this.right = right;
        }
        
        public override SyntaxType type => SyntaxType.AssignmentExpression;
    }
}