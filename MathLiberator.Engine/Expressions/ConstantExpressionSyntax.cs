using System;

namespace MathLiberator.Engine.Expressions
{
    public class ConstantExpressionSyntax<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public TNumber Value { get; }

        public ConstantExpressionSyntax(TNumber value)
        {
            Value = value;
        }

        public override String? ToString() => Value.ToString();
    }
}