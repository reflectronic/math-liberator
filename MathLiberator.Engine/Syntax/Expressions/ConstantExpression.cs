using System;

namespace MathLiberator.Syntax.Expressions
{
    public class ConstantExpression<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public TNumber Value { get; }

        public ConstantExpression(TNumber value)
        {
            Value = value;
        }

        public override String ToString() => Value.ToString()!;
    }
}