using System.Collections.Immutable;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class BlockStatement : Statement
    {
        public readonly Token lBrace;
        public readonly ImmutableArray<Statement> statements;
        public readonly Token rBrace;

        public BlockStatement(Token lBrace, ImmutableArray<Statement> statements, Token rBrace)
        {
            this.lBrace = lBrace;
            this.statements = statements;
            this.rBrace = rBrace;
        }

        public override SyntaxType type => SyntaxType.BlockStatement;
    }
}