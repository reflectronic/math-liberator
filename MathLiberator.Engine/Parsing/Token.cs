using System;

namespace MathLiberator.Engine.Parsing
{
    public ref struct Token<TNumber> where TNumber : unmanaged
    {
        public Token(ReadOnlySpan<char> stringValue)
        {
            TokenType = TokenType.Identifier;
            StringValue = stringValue;
            NumericValue = default;
            OperatorValue = default;
        }
        
        public Token(TNumber numericValue)
        {
            TokenType = TokenType.Number;
            NumericValue = numericValue;
            StringValue = default;
            OperatorValue = default;
        }

        public Token(OperatorType op)
        {
            TokenType = TokenType.Operator;
            OperatorValue = op;
            NumericValue = default;
            StringValue = default;
        }

        // TODO: Source location

        public TokenType TokenType { get; }
        public ReadOnlySpan<char> StringValue { get; }
        public TNumber? NumericValue { get; }
        public OperatorType? OperatorValue { get; }

        public override String? ToString() =>
            $@"<{TokenType} {TokenType switch
            {
                TokenType.Identifier => StringValue.ToString(),
                TokenType.Number => NumericValue.ToString(),
                TokenType.Operator => OperatorValue.ToString()
            }}>";
    }
}