using System;
using System.Buffers;

namespace MathLiberator.Syntax.Expressions
{
    public class IdentifierExpression<TNumber> : ExpressionSyntax<TNumber> 
        where TNumber : unmanaged
    {
        public ReadOnlySequence<Char> Identifier { get; }

        public IdentifierExpression(ReadOnlySequence<Char> identifier)
        {
            Identifier = identifier;
            Kind = SyntaxKind.Identifier;
        }

        public override String? ToString() => Identifier.ToString();

    }
}