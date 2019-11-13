using System;

namespace MathLiberator.Syntax.Expressions
{
    public class BinaryExpression<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public BinaryExpression(ExpressionSyntax<TNumber> left, SyntaxKind @operator, ExpressionSyntax<TNumber> right, SyntaxKind kind)
        {
            Left = left;
            Right = right;
            Operator = @operator;
            Kind = kind;
        }
        
        public ExpressionSyntax<TNumber> Left { get; }
        public SyntaxKind Operator { get; }
        public ExpressionSyntax<TNumber> Right { get; }
        public override String? ToString() => $"{Left} {Operator} {Right}";
    }
}