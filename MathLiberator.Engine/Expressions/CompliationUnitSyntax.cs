using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class CompilationUnitSyntax<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public ImmutableArray<ExpressionSyntax<TNumber>> Statements { get; }

        public CompilationUnitSyntax(ImmutableArray<ExpressionSyntax<TNumber>> statements)
        {
            Statements = statements;
        }
        
        public override String? ToString() => string.Join(Environment.NewLine, Statements);
    }
}