using System;

namespace MathLiberator.Engine.Syntax.Expressions
{
    public class BinaryExpressionSyntax<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public BinaryExpressionSyntax(ExpressionSyntax<TNumber> left, SyntaxKind @operator, ExpressionSyntax<TNumber> right)
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