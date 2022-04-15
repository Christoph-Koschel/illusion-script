namespace IllusionScript.Runtime.Parsing
{
    public enum SyntaxType
    {
        BadToken,
        WhiteSpaceToken,
        EOFToken,
        
        NumberToken,
        
        PlusToken,
        MinusToken,
        StarToken,
        DoubleStarToken,
        SlashToken,
        PercentToken,
        
        LParenToken,
        RParenToken,
        
        
        BinaryExpression,
        NumberExpression,
        ParenExpression,
        UnaryExpression,

    }
}