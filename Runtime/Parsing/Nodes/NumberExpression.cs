using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class NumberExpression : Expression
    {
        public readonly Token number;

        public NumberExpression(Token number)
        {
            this.number = number;
        }
        
        public override SyntaxType type => SyntaxType.NumberExpression;
    }
}