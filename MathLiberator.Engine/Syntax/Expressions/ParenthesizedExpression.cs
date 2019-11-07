using System;

namespace MathLiberator.Syntax.Expressions
{
    public class ParenthesizedExpression<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public ExpressionSyntax<TNumber> Expression { get; }

        public ParenthesizedExpression(ExpressionSyntax<TNumber> expression)
        {
            Expression = expression;
        }

        public override String? ToString() => $"({Expression})";
    }
}