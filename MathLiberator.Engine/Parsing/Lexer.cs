using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace MathLiberator.Engine.Parsing
{
    public ref struct Lexer<TNumber> where TNumber : unmanaged
    {
        SequenceReader<Char> reader;
        
        public Lexer(SequenceReader<Char> reader)
        {
            this.reader = reader;
        }

        public Token<TNumber> Lex()
        {
            start:
            while (!reader.End)
            {
                reader.TryAdvanceTo('\n');
                reader.TryPeek(out var c);

                switch (c)
                {
                    case '#':
                        reader.TryAdvanceTo('\n');
                        // ELSE: Advance to end
                        goto start; // Avoid recursion
                    case '{':    
                        reader.Advance(1);
                        return new Token<TNumber>(OperatorType.OpenBrace);
                    case '}':
                        reader.Advance(1);
                        return new Token<TNumber>(OperatorType.CloseBrace);
                    case '[':
                        reader.Advance(1);
                        return new Token<TNumber>(OperatorType.OpenBracket);
                    case ']':
                        reader.Advance(1);
                        return new Token<TNumber>(OperatorType.CloseBracket);
                    case '=':
                        reader.Advance(1);
                        return new Token<TNumber>(OperatorType.Equals);
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
            OperatorType v;
            if (reader.IsNext('='))
            {
                reader.Advance(1);
                v = OperatorType.ColonEquals;
            }
            else
            {
                v = OperatorType.Colon;
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
                '>' => inclusive ? OperatorType.GreaterThanEquals : OperatorType.GreaterThan,
                '<' => inclusive ? OperatorType.LessThanEquals : OperatorType.LessThan
            });
        }

        Token<TNumber> LexOperator()
        {
            reader.TryRead(out var c);
            if (reader.TryPeek(out var e) && e == '=')
            {
                reader.Advance(1);
                return new Token<TNumber>(c switch
                {
                    '+' => OperatorType.PlusEquals,
                    '-' => OperatorType.MinusEquals,
                    '*' => OperatorType.AsteriskEquals,
                    '/' => OperatorType.SlashEquals
                });
            }
            else
            {
                return new Token<TNumber>(c switch
                {
                    '+' => OperatorType.Plus,
                    '-' => OperatorType.Minus,
                    '*' => OperatorType.Asterisk,
                    '/' => OperatorType.Slash
                });
            }
        }

        ReadOnlySpan<Char> TryReadAny(ReadOnlySpan<Char> values)
        {
            var startingPosition = reader.Position;
            while (reader.IsNext(values))
            {
                reader.Advance(1);
            }

            var sequence = reader.Sequence.Slice(startingPosition, reader.Position);

            return sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray();
        }
        
        Token<TNumber> LexInteger()
        {
            var span = TryReadAny("012345789.");

            if (typeof(TNumber) == typeof(Single))
            {
                Single.TryParse(span, out var num);
                return new Token<TNumber>((TNumber) (Object) num);
            }
            
            if (typeof(TNumber) == typeof(Double))
            {
                Double.TryParse(span, out var num);
                return new Token<TNumber>((TNumber) (Object) num);
            }
            
            if (typeof(TNumber) == typeof(Decimal))
            {
                Decimal.TryParse(span, out var num);
                return new Token<TNumber>((TNumber) (Object) num);
            }
            
            throw new ArgumentException($"Unsupported type {typeof(TNumber)}", nameof(TNumber));
        }
    }
}