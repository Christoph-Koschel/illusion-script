using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class CallExpression : Expression
    {
        public readonly Token identifier;
        public readonly Token lParen;
        public readonly Node[] arguments;
        public readonly Token rParen;

        public CallExpression(Token identifier, Token lParen, Node[] arguments, Token rParen)
        {
            this.identifier = identifier;
            this.lParen = lParen;
            this.arguments = arguments;
            this.rParen = rParen;
        }
        
        public override SyntaxType type => SyntaxType.CallExpression;
    }
}