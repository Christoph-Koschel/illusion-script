using System.Collections.Generic;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public sealed class NameExpression : Expression
    {
        public override SyntaxType type => SyntaxType.NameExpression;
        public readonly Token identifier;

        public NameExpression(Token identifier)
        {
            this.identifier = identifier;
        }
    }
}