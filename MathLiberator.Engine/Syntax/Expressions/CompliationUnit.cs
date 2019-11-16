using System;
using System.Collections.Immutable;

namespace MathLiberator.Syntax.Expressions
{
    public class CompilationUnit<TNumber> : ExpressionSyntax<TNumber>
        where TNumber : unmanaged
    {
        public ImmutableArray<ExpressionSyntax<TNumber>> Statements { get; }

        public CompilationUnit(ImmutableArray<ExpressionSyntax<TNumber>> statements)
        {
            Statements = statements;
        }
        
        public override String? ToString() => string.Join(Environment.NewLine, Statements);
    }
}