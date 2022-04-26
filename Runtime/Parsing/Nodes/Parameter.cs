using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public sealed class Parameter : Node
    {
        public readonly Token identifier;
        public readonly TypeClause typeClause;

        public Parameter(Token identifier, TypeClause typeClause)
        {
            this.identifier = identifier;
            this.typeClause = typeClause;
        }

        public override SyntaxType type => SyntaxType.Parameter;
    }
}