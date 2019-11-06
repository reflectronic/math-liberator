using System;
using System.Buffers;
using System.Runtime.InteropServices;
using MathLiberator.Engine.Expressions;

namespace MathLiberator.Engine.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct Parser<TNumber>
        where TNumber : unmanaged
    {
        Lexer<TNumber> lexer;
        
        public Parser(ReadOnlySequence<Char> sequence)
        {
            lexer = new Lexer<TNumber>(new SequenceReader<Char>(sequence));
        }
        
        public MLExpression<TNumber> Parse()
        {
            lexer.Lex();
            ref var z = ref lexer.Current;
            
            switch (z.TokenType)
            {
                case TokenType.EOF:
                    break;
                case TokenType.Identifier:
                    break;
                case TokenType.Number:   
                    // Invalid
                    break;
                case TokenType.Operator:
                    // Check for minus
                    break;
            }

            return default;
        }

        static Int32 GetUnaryOperatorPrecedence(OperatorType op)
        {
            switch (op)
            {
                case OperatorType.Plus:
                case OperatorType.Minus:
                    return 6;
                default:
                    return 0;
            }
        }

        static Int32 GetOperatorPrecedence(OperatorType op)
        {
            switch (op)
            {
                case OperatorType.Asterisk:
                case OperatorType.Slash:
                    return 5;
                case OperatorType.Plus:
                case OperatorType.Minus:
                    return 4;
                case OperatorType.GreaterThan:
                case OperatorType.LessThan:
                case OperatorType.GreaterThanEquals:
                case OperatorType.LessThanEquals:
                    return 3;
                case OperatorType.Equals:
                    return 1;
                default:
                    return 0;

            }
        }
    }
}