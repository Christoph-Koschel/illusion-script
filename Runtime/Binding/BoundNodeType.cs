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
        ErrorExpression,
        ConversionExpression,
        CompoundAssignmentExpression,
        CallExpression,
        
        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement
    }
}