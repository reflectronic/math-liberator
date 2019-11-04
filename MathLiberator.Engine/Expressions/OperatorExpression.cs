using System;
using System.Linq.Expressions;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class OperatorExpression<TNumber> : MLExpression<TNumber>
        where TNumber : unmanaged
    {
        public OperatorExpression(Expression<TNumber> left, Expression<TNumber> right, OperatorType @operator)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }


        public Expression<TNumber> Left { get; }
        public Expression<TNumber> Right { get; }

        public TNumber Evaluate()
        {
            // TODO: Fold constant expressions
            throw new NotImplementedException();
        }
        
        public OperatorType Operator { get; }
    }
}