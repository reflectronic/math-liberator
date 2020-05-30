using System;
using System.Buffers;
using System.Globalization;

namespace MathLiberator.Syntax.Expressions
{
    public class IdentifierExpression<TNumber> : ExpressionSyntax<TNumber> 
        where TNumber : unmanaged
    {
        public ReadOnlySequence<Char> Identifier { get; }

        public IdentifierExpression(ReadOnlySequence<Char> identifier)
        {
            Identifier = identifier;
        }

        String? str;

        public override String ToString() => str ??= Identifier.ToString();
    }
}