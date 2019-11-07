using System;

namespace MathLiberator.Engine.Syntax.Expressions
{
    public class MutationSyntax<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public ExpressionSyntax<TNumber> Source { get; }
        
        public SyntaxKind Operator { get; }
        
        public ExpressionSyntax<TNumber> Expression { get; }


        public MutationSyntax(ExpressionSyntax<TNumber> source, SyntaxKind @operator, ExpressionSyntax<TNumber> expression)
        {
            Source = source;
            Operator = @operator;
            Expression = expression;
        }

        public override String? ToString() => $"{Source} {Operator} {Expression}";
    }
}