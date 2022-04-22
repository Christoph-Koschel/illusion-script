﻿using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes.Expressions;

namespace IllusionScript.Runtime.Parsing.Nodes.Statements
{
    public sealed class IfStatement : Statement
    {
        public readonly Token keyword;
        private readonly Token lParen;
        public readonly Expression condition;
        private readonly Token rParen;
        public readonly Statement body;
        public readonly ElseClause elseClause;

        public IfStatement(Token keyword, Token lParen, Expression condition, Token rParen, Statement body, ElseClause elseClause)
        {
            this.keyword = keyword;
            this.lParen = lParen;
            this.condition = condition;
            this.rParen = rParen;
            this.body = body;
            this.elseClause = elseClause;
        }

        public override SyntaxType type => SyntaxType.IfStatement;
    }
}