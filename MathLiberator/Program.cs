using System;
using System.Buffers;
using MathLiberator.Engine.Parsing;

namespace MathLiberator
{
    static class Program
    {
        static void Main(String[] args)
        {
            var test = @"# a falling object with a mass of 10 kilograms (without air resistance)
g := -9.81
mass := 10
gravity := g * mass
acceleration := gravity / mass

velocity = 0
height = 100

[0:1:height>=0]
{
    velocity += acceleration
    height += velocity
}";
            
            var lxr = new Lexer<Double>(new SequenceReader<Char>(new ReadOnlySequence<Char>(test.AsMemory())));
            while (lxr.Lex() is var t && t.TokenType != TokenType.EOF)
            {
                Console.WriteLine(t.ToString());
            }
        }
    }
}