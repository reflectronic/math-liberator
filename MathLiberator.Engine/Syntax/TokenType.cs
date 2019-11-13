namespace MathLiberator.Syntax
{
    public enum SyntaxKind
    {
        // Token Kinds
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
        
        // Expression
        CompilationUnit,
        Constant,
        Model,
        Parenthesized,
        Unary,

        // Binary Expression Kinds
        Arithmetic,
        State,
        Mutation
        
    }
}