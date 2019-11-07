using System;

namespace MathLiberator.Syntax
{
    public static class SyntaxFacts
    {
        public static Int32 GetUnaryOperatorPrecedence(this SyntaxKind op)
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

        public static Int32 GetOperatorPrecedence(this SyntaxKind op)
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