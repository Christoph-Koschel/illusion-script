using System;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Runtime.Binding.Nodes.Expressions
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public readonly VariableSymbol variableSymbol;
        public readonly BoundExpression expression;

        public BoundAssignmentExpression(VariableSymbol variableSymbol, BoundExpression expression)
        {
            this.variableSymbol = variableSymbol;
            this.expression = expression;
        }

        public override BoundNodeType boundType => BoundNodeType.AssignmentExpression;
        public override TypeSymbol type => expression.type;
    }
}