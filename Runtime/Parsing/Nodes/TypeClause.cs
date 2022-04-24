using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public sealed class TypeClause : Node
    {
        public readonly Token colon;
        public readonly Token identifier;

        public TypeClause(Token colon, Token identifier)
        {
            this.colon = colon;
            this.identifier = identifier;
        }

        public override SyntaxType type => SyntaxType.TypeClause;
    }
}