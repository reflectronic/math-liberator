using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MathLiberator.Syntax.Expressions;

namespace MathLiberator.Syntax
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
        
        public CompilationUnit<TNumber> ParseCompilationUnit()
        {
            lexer.Lex();
            ref var current = ref lexer.Current;
            var builder = ImmutableArray.CreateBuilder<ExpressionSyntax<TNumber>>();

            while (current.Kind != SyntaxKind.EndOfFile)
            {
                builder.Add(ParseStatement());
            }
            
            return new CompilationUnit<TNumber>(builder.ToImmutable());
        }

        /// <summary>
        /// Parses a statement. A statement is either a model statement or a state statement.
        /// </summary>
        ExpressionSyntax<TNumber> ParseStatement()
        {
            ref var current = ref lexer.Current;
            return current.Kind switch
            {
                SyntaxKind.OpenBracket => ParseModelStatement(),
                SyntaxKind.Identifier => ParseStateExpression(),
                _ => default,
            };
        }
        
        
        /// <summary>
        /// Parses the model statement.
        /// </summary>
        /// <remarks>
        /// It is of the form:
        /// <code>
        /// [start:step:condition]
        /// {
        ///    multiple model_expression
        /// }
        /// </code>
        ///
        /// model_expression takes the form
        /// <code>
        /// state (*|+|etc.)= expr
        /// </code>
        /// </remarks>
        /// <returns></returns>
        ModelExpression<TNumber> ParseModelStatement()
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

            return new ModelExpression<TNumber>(start, step, (BinaryExpression<TNumber>) condition, builder.ToImmutable());
        }

        /// <summary>
        /// Parse an expression of the form <c>identifier (=|:=|+=|etc.) expression</c>. 
        /// </summary>
        /// <remarks>
        /// THis method is used to parse both state statements and model expressions, so we need to deal with both.
        /// </remarks>
        ExpressionSyntax<TNumber> ParseStateExpression()
        {
            var id = ParseIdentifier();
            ref var op = ref lexer.Current;
            var kind = op.Kind;
            lexer.Lex();
            
            var constant = false;
            
            // This method i
            switch (kind)
            {
                case SyntaxKind.ColonEquals:
                    constant = true;
                    goto case SyntaxKind.Equals;
                case SyntaxKind.Equals:
                    var expr = ParseExpression();
                    return new BinaryExpression<TNumber>(id, kind, expr, constant ? SyntaxKind.Constant : SyntaxKind.State);
                case SyntaxKind.PlusEquals:
                case SyntaxKind.MinusEquals:
                case SyntaxKind.AsteriskEquals:
                case SyntaxKind.SlashEquals:
                    return new BinaryExpression<TNumber>(id, kind, ParseExpression(), SyntaxKind.Mutation);
                default:
                    return default;
            }
        }
        
        /// <summary>
        /// Parses an entire expression. It handles unary and binary operations & their precedence.
        /// </summary>
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
                left = new UnaryExpression<TNumber>(operand, operatorToken.Kind);
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
                left = new BinaryExpression<TNumber>(left, kind, right, SyntaxKind.Arithmetic);
            }

            return left;
        }

        /// <summary>
        /// Parses the next one-element expression.
        /// </summary>
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

        IdentifierExpression<TNumber> ParseIdentifier()
        {
            MatchToken(SyntaxKind.Identifier, out var id);
            return new IdentifierExpression<TNumber>(id.StringValue);
        }
        
        ConstantExpression<TNumber> ParseNumberLiteral()
        {
            MatchToken(SyntaxKind.Number, out var num);
            return new ConstantExpression<TNumber>(num.NumericValue);
        }

        /// <summary>
        /// Parses an expression of the form (expr)
        /// </summary>
        ExpressionSyntax<TNumber> ParseParenthesizedExpression()
        {
            MatchToken(SyntaxKind.OpenParenthesis, out _);
            var expression = ParseExpression();
            MatchToken(SyntaxKind.CloseParenthesis, out _);
            return new UnaryExpression<TNumber>(expression, SyntaxKind.Parenthesized);
        }
        
        /// <summary>
        /// Asserts that the current token is of a specific type and advances to the next token.
        /// </summary>
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
                // TODO: Diagnostics
            }
        }
    }
}