using System.Collections.Immutable;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes
{
    public class CompilationUnit : Node
    {
        public readonly ImmutableArray<Member> members;
        public readonly Token endOfFileToken;

        public CompilationUnit(ImmutableArray<Member> members, Token endOfFileToken)
        {
            this.members = members;
            this.endOfFileToken = endOfFileToken;
        }

        public override SyntaxType type => SyntaxType.CompilationUnit;
    }
}