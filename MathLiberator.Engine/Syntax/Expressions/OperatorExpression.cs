using System;

namespace MathLiberator.Syntax.Expressions
{
    public class BinaryExpression<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public BinaryExpression(ExpressionSyntax<TNumber> left, SyntaxKind @operator, ExpressionSyntax<TNumber> right)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }


        public ExpressionSyntax<TNumber> Left { get; }
        public ExpressionSyntax<TNumber> Right { get; }

        public TNumber Evaluate()
        {
            // TODO: Fold constant expressions
            throw new NotImplementedException();
        }
        
        public SyntaxKind Operator { get; }
        
        public override String? ToString() => $"{Left} {Operator} {Right}";
    }
}