using System;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class UnaryExpressionSyntax<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public UnaryExpressionSyntax(ExpressionSyntax<TNumber> operand, SyntaxKind @operator)
        {
            Operand = operand;
            Operator = @operator;
        }

        public ExpressionSyntax<TNumber> Operand { get; }

        public TNumber Evaluate()
        {
            // TODO: Fold constant expressions
            throw new NotImplementedException();
        }
        
        public SyntaxKind Operator { get; }
        
        public override String? ToString() => $"{Operator}{Operand}";
    }
}