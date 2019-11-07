using System;
using System.Collections.Immutable;

namespace MathLiberator.Engine.Syntax.Expressions
{
    public class ModelExpressionSyntax<TNumber> : ExpressionSyntax<TNumber> 
        where TNumber : unmanaged
    {
        public ExpressionSyntax<TNumber> Start { get; }
        public ExpressionSyntax<TNumber> Step { get; }
        public ExpressionSyntax<TNumber> Condition { get; }
        public ImmutableArray<ExpressionSyntax<TNumber>> ModelStatements { get; }

        public ModelExpressionSyntax(ExpressionSyntax<TNumber> start, ExpressionSyntax<TNumber> step, ExpressionSyntax<TNumber> condition, ImmutableArray<ExpressionSyntax<TNumber>> modelStatements)
        {
            Start = start;
            Step = step;
            Condition = condition;
            ModelStatements = modelStatements;
        }

        public override String? ToString() => @$"[{Start}:{Step}:{Condition}]
{{
    {String.Join(Environment.NewLine, ModelStatements)}
}}";
    }
}