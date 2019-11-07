using System;

namespace MathLiberator.Engine.Syntax.Expressions
{
    public class ParenthesizedExpressionSyntax<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public ExpressionSyntax<TNumber> Expression { get; }

        public ParenthesizedExpressionSyntax(ExpressionSyntax<TNumber> expression)
        {
            Expression = expression;
        }

        public override String? ToString() => $"({Expression})";
    }
}