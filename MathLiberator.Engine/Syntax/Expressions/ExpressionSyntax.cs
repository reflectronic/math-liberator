using System;

namespace MathLiberator.Syntax.Expressions
{
    public abstract class ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public SyntaxKind Kind { get; protected set; }

        public virtual TNumber? TryEvaluate()
        {
            throw new NotSupportedException("Cannot evaluate this expression type");
        }
    }
}
