using System;
using System.Collections.Immutable;

namespace MathLiberator.Syntax.Expressions
{
    public class ModelExpression<TNumber> : ExpressionSyntax<TNumber> 
        where TNumber : unmanaged
    {
        public ExpressionSyntax<TNumber> Start { get; }
        public ExpressionSyntax<TNumber> Step { get; }
        public ExpressionSyntax<TNumber> Condition { get; }
        public ImmutableArray<ExpressionSyntax<TNumber>> ModelStatements { get; }

        public ModelExpression(ExpressionSyntax<TNumber> start, ExpressionSyntax<TNumber> step, ExpressionSyntax<TNumber> condition, ImmutableArray<ExpressionSyntax<TNumber>> modelStatements)
        {
            Start = start;
            Step = step;
            Condition = condition;
            ModelStatements = modelStatements;
            Kind = SyntaxKind.Model;
        }

        public override String? ToString() => @$"[{Start}:{Step}:{Condition}]
{{
    {String.Join(Environment.NewLine, ModelStatements)}
}}";
    }
}