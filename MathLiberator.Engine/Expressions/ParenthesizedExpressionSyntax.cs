using System;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
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