namespace IllusionScript.Runtime.Binding
{
    public enum BoundNodeType
    {
        // Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
        CallExpression,
        ErrorExpression,
        
        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        GotoStatement,
        LabelStatement,
        ConditionalGotoStatement,
    }
}