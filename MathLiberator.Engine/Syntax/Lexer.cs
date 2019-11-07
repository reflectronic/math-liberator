using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace MathLiberator.Engine.Syntax
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct Lexer<TNumber> where TNumber : unmanaged
    {
        SequenceReader<Char> reader;

        public Token<TNumber> Current;
        
        public Lexer(SequenceReader<Char> reader)
        {
            this.reader = reader;
            Current = default;
        }

        public void Lex()
        {
            Current = GetToken();
        }

        Token<TNumber> GetToken()
        {
            start:
            while (!reader.End)
            {
                reader.AdvancePastAny(" \r\n");
                reader.TryPeek(out var c);

                switch (c)
                {
                    case '#':
                        reader.TryAdvanceTo('\n');
                        // ELSE: Advance to end
                        goto start; // Avoid recursion
                    case '{':    
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.OpenBrace);
                    case '}':
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.CloseBrace);
                    case '[':
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.OpenBracket);
                    case ']':
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.CloseBracket);
                    case '(':
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.OpenParenthesis);
                    case ')':
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.CloseParenthesis);
                    case '=':
                        reader.Advance(1);
                        return new Token<TNumber>(SyntaxKind.Equals);
                    case '+': case '-': case '*': case '/':
                        return LexOperator();
                    case ':':
                        return LexColon();
                    case '<': case '>':
                        return LexRelationalOperator();
                    case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9': case '.':
                        return LexInteger();
                    default:
                        return LexIdentifier();
                }
            }

            return default;
        }

        Token<TNumber> LexIdentifier()
        {
            var span = TryReadAny("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_");
            return new Token<TNumber>(span);
        }

        Token<TNumber> LexColon()
        {
            reader.Advance(1);
            SyntaxKind v;
            if (reader.IsNext('='))
            {
                reader.Advance(1);
                v = SyntaxKind.ColonEquals;
            }
            else
            {
                v = SyntaxKind.Colon;
            }

            return new Token<TNumber>(v);
        }

        Token<TNumber> LexRelationalOperator()
        {
            reader.TryRead(out var c);
            var inclusive = reader.IsNext('=');
            
            if (inclusive) reader.Advance(1);

            return new Token<TNumber>(c switch
            {
                '>' => inclusive ? SyntaxKind.GreaterThanEquals : SyntaxKind.GreaterThan,
                '<' => inclusive ? SyntaxKind.LessThanEquals : SyntaxKind.LessThan
            });
        }

        Token<TNumber> LexOperator()
        {
            reader.TryRead(out var c);
            if (reader.IsNext('='))
            {
                reader.Advance(1);
                return new Token<TNumber>(c switch
                {
                    '+' => SyntaxKind.PlusEquals,
                    '-' => SyntaxKind.MinusEquals,
                    '*' => SyntaxKind.AsteriskEquals,
                    '/' => SyntaxKind.SlashEquals
                });
            }
            else
            {
                return new Token<TNumber>(c switch
                {
                    '+' => SyntaxKind.Plus,
                    '-' => SyntaxKind.Minus,
                    '*' => SyntaxKind.Asterisk,
                    '/' => SyntaxKind.Slash
                });
            }
        }

        ReadOnlySequence<Char> TryReadAny(ReadOnlySpan<Char> values)
        {
            var startingPosition = reader.Position;
            while (reader.TryPeek(out var c) && values.Contains(c))
            {
                reader.Advance(1);
            }

            var sequence = reader.Sequence.Slice(startingPosition, reader.Position);

            return sequence;
        }

        ReadOnlySpan<Char> SequenceToSpan(in ReadOnlySequence<Char> sequence) => sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray();

        Token<TNumber> LexInteger()
        {
            var span = TryReadAny("012345789.");

            if (typeof(TNumber) == typeof(Single))
            {
                Single.TryParse(SequenceToSpan(span), out var num);
                return new Token<TNumber>((TNumber) (Object) num);
            }
            
            if (typeof(TNumber) == typeof(Double))
            {
                Double.TryParse(SequenceToSpan(span), out var num);
                return new Token<TNumber>((TNumber) (Object) num);
            }
            
            if (typeof(TNumber) == typeof(Decimal))
            {
                Decimal.TryParse(SequenceToSpan(span), out var num);
                return new Token<TNumber>((TNumber) (Object) num);
            }
            
            throw new ArgumentException($"Unsupported type {typeof(TNumber)}", nameof(TNumber));
        }
    }
}