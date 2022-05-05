﻿using IllusionScript.Runtime.Extension;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes.Statements;

namespace IllusionScript.Runtime.Parsing.Nodes.Members
{
    public sealed class FunctionDeclarationMember : Member
    {
        public readonly Token functionKeyword;
        public readonly Token identifier;
        public readonly Token lParen;
        public readonly SeparatedSyntaxList<Parameter> parameters;
        public readonly Token rParen;
        public readonly TypeClause typeClause;
        public readonly BlockStatement body;

        public FunctionDeclarationMember(
            SyntaxTree syntaxTree, Token functionKeyword,
            Token identifier, Token lParen,
            SeparatedSyntaxList<Parameter> parameters, Token rParen,
            TypeClause typeClause, BlockStatement body
        ) : base(syntaxTree)
        {
            this.functionKeyword = functionKeyword;
            this.identifier = identifier;
            this.lParen = lParen;
            this.parameters = parameters;
            this.rParen = rParen;
            this.typeClause = typeClause;
            this.body = body;
        }

        public override SyntaxType type => SyntaxType.FunctionDeclarationMember;
    }
}