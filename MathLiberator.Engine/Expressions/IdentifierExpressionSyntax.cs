using System;
using System.Buffers;
using System.Linq.Expressions;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class IdentifierExpressionSyntax<TNumber> : ExpressionSyntax<TNumber> 
        where TNumber : unmanaged
    {
        public ReadOnlySequence<Char> Identifier { get; }

        public IdentifierExpressionSyntax(ReadOnlySequence<Char> identifier)
        {
            Identifier = identifier;
        }

        public override String? ToString() => Identifier.ToString();

    }
}