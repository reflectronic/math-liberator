using System;

namespace MathLiberator.Syntax.Expressions
{
    public class MutationExpression<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public ExpressionSyntax<TNumber> Source { get; }
        
        public SyntaxKind Operator { get; }
        
        public ExpressionSyntax<TNumber> Expression { get; }


        public MutationExpression(ExpressionSyntax<TNumber> source, SyntaxKind @operator, ExpressionSyntax<TNumber> expression)
        {
            Source = source;
            Operator = @operator;
            Expression = expression;
        }

        public override String? ToString() => $"{Source} {Operator} {Expression}";
    }
}