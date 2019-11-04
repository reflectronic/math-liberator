namespace MathLiberator.Engine.Parsing
{
    public enum TokenType
    {
        EOF,
        Identifier,
        Number,
        Operator,
    }
    
    public enum OperatorType
    {
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        
        Equals,
        Colon,
        Plus,
        Minus,
        Asterisk,
        Slash,

        PlusEquals,
        MinusEquals,
        AsteriskEquals,
        SlashEquals,
        ColonEquals,

        GreaterThan,
        GreaterThanEquals,
        LessThan,
        LessThanEquals,
    }
}