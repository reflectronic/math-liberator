using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
            ref var current = ref lexer.Current;
            

            return default;
        }

        MLExpression<TNumber> ParsePrimaryExpression()
        {
            ref var current = ref lexer.Current;
            return default;
        }

        static Int32 GetUnaryOperatorPrecedence(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 6;
                default:
                    return 0;
            }
        }

        static Int32 GetOperatorPrecedence(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.Asterisk:
                case SyntaxKind.Slash:
                    return 5;
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 4;
                case SyntaxKind.GreaterThan:
                case SyntaxKind.LessThan:
                case SyntaxKind.GreaterThanEquals:
                case SyntaxKind.LessThanEquals:
                    return 3;
                case SyntaxKind.Equals:
                    return 1;
                default:
                    return 0;

            }
        }
    }
}