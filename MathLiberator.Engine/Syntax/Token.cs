using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace MathLiberator.Syntax
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Token<TNumber> where TNumber : unmanaged
    {
        public Token(ReadOnlySequence<char> stringValue)
        {
            Kind = SyntaxKind.Identifier;
            StringValue = stringValue;
            NumericValue = default;
        }
        
        public Token(TNumber numericValue)
        {
            Kind = SyntaxKind.Number;
            NumericValue = numericValue;
            StringValue = default;
        }

        public Token(SyntaxKind op)
        {
            Kind = op;
            NumericValue = default;
            StringValue = default;
        }

        // TODO: Source location

        public ReadOnlySequence<char> StringValue { get; }
        public TNumber? NumericValue { get; }
        public SyntaxKind Kind { get; }

        public override String? ToString() =>
            $@"<{Kind}{Kind switch
            {
                SyntaxKind.Identifier => $" {StringValue.ToString()}",
                SyntaxKind.Number => $" {NumericValue}",
                _ => string.Empty
            }}>";
    }
}