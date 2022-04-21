using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class VariableDeclarationStatement : Statement
    {
        public readonly Token keyword;
        public readonly Token identifier;
        public readonly Token equalsToken;
        public readonly Expression initializer;

        public VariableDeclarationStatement(Token keyword, Token identifier, Token equalsToken, Expression initializer)
        {
            this.keyword = keyword;
            this.identifier = identifier;
            this.equalsToken = equalsToken;
            this.initializer = initializer;
        }
        
        public override SyntaxType type => SyntaxType.VariableDeclarationStatement;
        
    }
}