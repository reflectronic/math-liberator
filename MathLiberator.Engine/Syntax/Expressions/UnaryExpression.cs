using System;

namespace MathLiberator.Syntax.Expressions
{
    public class UnaryExpression<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public UnaryExpression(ExpressionSyntax<TNumber> operand, SyntaxKind @operator)
        {
            Operand = operand;
            Operator = @operator;
        }

        public ExpressionSyntax<TNumber> Operand { get; }

        public TNumber Evaluate()
        {
            return default;
        }
        
        public SyntaxKind Operator { get; }
        
        public override String? ToString() => $"{Operator} ({Operand})";
    }
}