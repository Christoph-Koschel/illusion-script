using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    public sealed class BoundExpressionStatement : BoundStatement
    {
        public readonly BoundExpression expression;

        public BoundExpressionStatement(BoundExpression expression)
        {
            this.expression = expression;
        }

        public override BoundNodeType boundType => BoundNodeType.ExpressionStatement;
    }

    public sealed class BoundVariableDeclarationStatement : BoundStatement
    {
        public readonly VariableSymbol variable;
        public readonly BoundExpression initializer;

        public BoundVariableDeclarationStatement(VariableSymbol variable, BoundExpression initializer)
        {
            this.variable = variable;
            this.initializer = initializer;
        }

        public override BoundNodeType boundType => BoundNodeType.VariableDeclarationStatement;
    }
}