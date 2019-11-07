using System;
using System.Collections.Immutable;

namespace MathLiberator.Engine    .Syntax.Expressions
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