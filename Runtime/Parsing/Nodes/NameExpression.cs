using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class NameExpression : Expression
    {
        public readonly Token identifier;

        public NameExpression(Token identifier)
        {
            this.identifier = identifier;
        }
        
        public override SyntaxType type => SyntaxType.NameExpression;
    }
}