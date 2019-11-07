using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MathLiberator.Engine.Syntax.Expressions;

namespace MathLiberator.Engine.Syntax
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
                    return ParseStateExpression();
                default:
                    return default;
            }
        }

        ExpressionSyntax<TNumber> ParseStateExpression()
        {
            var id = ParseIdentifier();
            ref var op = ref lexer.Current;
            var kind = op.Kind;
            lexer.Lex();
            var constant = false;
            switch (kind)
            {
                case SyntaxKind.ColonEquals:
                    constant = true;
                    goto case SyntaxKind.Equals;
                case SyntaxKind.Equals:
                    var expr = ParseExpression();
                    return new StateExpressionSyntax<TNumber>(id.Identifier, expr, constant);
                case SyntaxKind.PlusEquals:
                case SyntaxKind.MinusEquals:
                case SyntaxKind.AsteriskEquals:
                case SyntaxKind.SlashEquals:
                    return new MutationSyntax<TNumber>(id, kind, ParseExpression());
                default:
                    return default;
            }
            
        }

        ExpressionSyntax<TNumber> ParseModelStatement()
        {
            MatchToken(SyntaxKind.OpenBracket, out _);
            var start = ParseExpression();
            MatchToken(SyntaxKind.Colon, out _);
            var step = ParseExpression();
            MatchToken(SyntaxKind.Colon, out _);
            var condition = ParseExpression();
            MatchToken(SyntaxKind.CloseBracket, out _);
            MatchToken(SyntaxKind.OpenBrace, out _);

            var builder = ImmutableArray.CreateBuilder<ExpressionSyntax<TNumber>>();

            ref var current = ref lexer.Current;
            while (current.Kind != SyntaxKind.CloseBrace && current.Kind != SyntaxKind.EndOfFile)
            {
                builder.Add(ParseStatement());
            }

            MatchToken(SyntaxKind.CloseBrace, out _);

            return new ModelExpressionSyntax<TNumber>(start, step, condition, builder.ToImmutable());
        }

        ExpressionSyntax<TNumber> ParseExpression(Int32 parentPrecedence = 0)
        {
            ExpressionSyntax<TNumber> left;
            ref var current = ref lexer.Current;
            var unaryPrecedence = current.Kind.GetUnaryOperatorPrecedence();
            if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
            {
                lexer.Lex();
                ref var operatorToken = ref lexer.Current;
                var operand = ParseExpression(unaryPrecedence);
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
                var precedence = kind.GetOperatorPrecedence();
                if (precedence is 0 || precedence <= parentPrecedence)
                    break;
                
                lexer.Lex();
                currentToken = ref lexer.Current;
                var right = ParseExpression(precedence);
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

        IdentifierExpressionSyntax<TNumber> ParseIdentifier()
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
            var expression = ParseExpression();
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
    }
}