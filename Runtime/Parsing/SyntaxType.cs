namespace IllusionScript.Runtime.Parsing
{
    public enum SyntaxType
    {
        // Special Tokens
        BadToken,
        WhiteSpaceToken,
        CommaToken,
        EOFToken,

        // Data Types
        NumberToken,
        StringToken,
        IdentifierToken,

        // Math operations
        PlusToken,
        PlusEqualsToken,

        MinusToken,
        MinusEqualsToken,

        StarToken,
        StarEqualsToken,
        DoubleStarToken,
        DoubleStarEqualsToken,

        SlashToken,
        SlashEqualsToken,

        PercentToken,
        PercentEqualsToken,

        AndEqualsToken,
        AndToken,

        SplitToken,
        SplitEqualsToken,

        HatToken,
        HatEqualsToken,

        DoubleGreaterToken,
        DoubleLessToken,

        // Logical Tokens
        LessToken,
        LessEqualsToken,
        GreaterToken,
        GreaterEqualsToken,
        DoubleEqualsToken,
        BangEqualsToken,

        // Unary Tokens
        BangToken,
        EqualsToken,
        DoubleAndToken,
        DoubleSplitToken,

        // Structure Tokens
        LParenToken,
        RParenToken,
        LBraceToken,
        RBraceToken,
        ColonToken,
        TildeToken,

        // Keywords
        FalseKeyword,
        TrueKeyword,
        BreakKeyword,
        ContinueKeyword,
        ElseKeyword,
        ForKeyword,
        FunctionKeyword,
        IfKeyword,
        LetKeyword,
        ConstKeyword,
        ReturnKeyword,
        ToKeyword,
        WhileKeyword,
        DoKeyword,

        // Expressions
        BinaryExpression,
        LiteralExpression,
        ParenExpression,
        UnaryExpression,
        AssignmentExpression,
        NameExpression,
        CallExpression,

        // Structure
        TypeClause,
        Parameter,
        CompilationUnit,

        // Statements
        BlockStatement,
        ExpressionStatement,
        FunctionDeclarationMember,
        VariableDeclarationStatement,
        IfStatement,
        ElseClause,
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        ContinueStatement,
        BreakStatement,
        
        // Members
        StatementMember,
    }
}