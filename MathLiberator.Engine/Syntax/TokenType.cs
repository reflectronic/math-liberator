namespace MathLiberator.Syntax
{
    public enum SyntaxKind
    {
        EndOfFile,
        Identifier,
        Number,
        
        OpenParenthesis,
        CloseParenthesis,
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