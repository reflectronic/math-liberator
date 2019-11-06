using System;
using System.Linq.Expressions;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class BinaryExpression<TNumber> : MLExpression<TNumber>
        where TNumber : unmanaged
    {
        public BinaryExpression(Expression<TNumber> left, Expression<TNumber> right, SyntaxKind @operator)
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
        
        public SyntaxKind Operator { get; }
    }
}