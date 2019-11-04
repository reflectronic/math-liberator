namespace MathLiberator.Engine.Expressions
{
    public class ConstantExpression<TNumber> : MLExpression<TNumber>
        where TNumber : unmanaged
    {
        public TNumber Value { get; }

        public ConstantExpression(TNumber value)
        {
            Value = value;
        }
    }
}