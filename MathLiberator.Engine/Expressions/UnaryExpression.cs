using System;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class UnaryExpression<TNumber> : MLExpression<TNumber>
        where TNumber : unmanaged
    {
        public UnaryExpression(MLExpression<TNumber> left, SyntaxKind @operator)
        {
            Left = left;
            Operator = @operator;
        }

        public MLExpression<TNumber> Left { get; }

        public TNumber Evaluate()
        {
            // TODO: Fold constant expressions
            throw new NotImplementedException();
        }
        
        public SyntaxKind Operator { get; }
    }
}