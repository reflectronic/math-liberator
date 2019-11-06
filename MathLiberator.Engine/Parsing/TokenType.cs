namespace MathLiberator.Engine.Parsing
{
    public enum SyntaxKind
    {
        EndOfFile,
        Identifier,
        Number,
        
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