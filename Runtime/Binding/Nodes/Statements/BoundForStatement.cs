using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Statements
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public readonly VariableSymbol variable;
        public readonly BoundExpression startExpression;
        public readonly BoundExpression endExpression;
        public readonly BoundStatement body;

        public BoundForStatement(VariableSymbol variable, BoundExpression startExpression, BoundExpression endExpression, BoundStatement body)
        {
            this.variable = variable;
            this.startExpression = startExpression;
            this.endExpression = endExpression;
            this.body = body;
        }

        public override BoundNodeType boundType => BoundNodeType.ForStatement;
    }
}