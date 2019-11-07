using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        
        public ExpressionSyntax<TNumber> Parse()
        {
            lexer.Lex();
            ref var current = ref lexer.Current;
            var builder = ImmutableArray.CreateBuilder<ExpressionSyntax<TNumber>>();

            while (current.Kind != SyntaxKind.EndOfFile)
            {
                builder.Add(ParseStatement());
            }
            
            return new CompilationUnitSyntax<TNumber>(builder.ToImmutable());
        }

        ExpressionSyntax<TNumber> ParseStatement()
        {
            ref var current = ref lexer.Current;
            switch (current.Kind)
            {
                case SyntaxKind.OpenBracket:
                    return ParseModelStatement();
                case SyntaxKind.Identifier:
                    return ParseStateStatement();
                default:
                    return default;
            }
        }

        ExpressionSyntax<TNumber> ParseStateStatement()
        {
            MatchToken(SyntaxKind.Identifier, out var id);
            ref var op = ref lexer.Current;
            var constant = op.Kind == SyntaxKind.ColonEquals;
            lexer.Lex();
            var expr = ParseOperatorExpression();
            
            return new StateExpressionSyntax<TNumber>(id.StringValue, expr, constant);
        }

        ExpressionSyntax<TNumber> ParseModelStatement()
        {
            MatchToken(SyntaxKind.OpenBracket, out _);
            var start = ParseOperatorExpression();
            MatchToken(SyntaxKind.Colon, out _);
            var step = ParseOperatorExpression();
            MatchToken(SyntaxKind.Colon, out _);
            var condition = ParseOperatorExpression();
            MatchToken(SyntaxKind.CloseBracket, out _);
            MatchToken(SyntaxKind.OpenBrace, out _);

            var builder = ImmutableArray.CreateBuilder<ExpressionSyntax<TNumber>>();

            ref var current = ref lexer.Current;
            while (current.Kind != SyntaxKind.CloseBrace && current.Kind != SyntaxKind.EndOfFile)
            {
                builder.Add(ParseStatement());
            }

            return new ModelExpressionSyntax<TNumber>(start, step, condition, builder.ToImmutable());
        }

        ExpressionSyntax<TNumber> ParseOperatorExpression(Int32 parentPrecedence = 0)
        {
            ExpressionSyntax<TNumber> left;
            ref var current = ref lexer.Current;
            var unaryPrecedence = GetUnaryOperatorPrecedence(current.Kind);
            if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
            {
                lexer.Lex();
                ref var operatorToken = ref lexer.Current;
                var operand = ParseOperatorExpression(unaryPrecedence);
                left = new UnaryExpressionSyntax<TNumber>(operand, operatorToken.Kind);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                ref var currentToken = ref lexer.Current;
                var kind = currentToken.Kind;
                var precedence = GetOperatorPrecedence(kind);
                if (precedence is 0 || precedence <= parentPrecedence)
                    break;
                
                lexer.Lex();
                currentToken = ref lexer.Current;
                var right = ParseOperatorExpression(precedence);
                left = new BinaryExpressionSyntax<TNumber>(left, kind, right);
            }

            return left;
        }

        ExpressionSyntax<TNumber> ParsePrimaryExpression()
        {
            ref var current = ref lexer.Current;

            switch (current.Kind)
            {
                case SyntaxKind.OpenParenthesis:
                    return ParseParenthesizedExpression();
                case SyntaxKind.Number:    
                    return ParseNumberLiteral();
                case SyntaxKind.Identifier:
                    return ParseIdentifier();
                default:
                    return default;
            }
        }

        ExpressionSyntax<TNumber> ParseIdentifier()
        {
            MatchToken(SyntaxKind.Identifier, out var id);
            return new IdentifierExpressionSyntax<TNumber>(id.StringValue);
        }

        ExpressionSyntax<TNumber> ParseNumberLiteral()
        {
            MatchToken(SyntaxKind.Number, out var num);
            return new ConstantExpressionSyntax<TNumber>(num.NumericValue.Value);
        }

        ExpressionSyntax<TNumber> ParseParenthesizedExpression()
        {
            MatchToken(SyntaxKind.OpenParenthesis, out _);
            var expression = ParseOperatorExpression();
            MatchToken(SyntaxKind.CloseParenthesis, out _);
            return new ParenthesizedExpressionSyntax<TNumber>(expression);
        }
        
        void MatchToken(SyntaxKind kind, out Token<TNumber> token)
        {
            ref var current = ref lexer.Current;
            if (current.Kind == kind)
            {
                token = Unsafe.AsRef(in lexer.Current);
                lexer.Lex();
            }
            else
            {
                token = default;
                Trace.Assert(false, "current.Kind is not equal to kind");
            }
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