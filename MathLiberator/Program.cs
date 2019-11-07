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
            while (true)
            {
                lxr.Lex();
                ref var token = ref lxr.Current;
                Console.WriteLine(token.ToString());
                if (token.Kind is SyntaxKind.EndOfFile) break;
            }
            
            Console.WriteLine("---Parsing---");
            var parser = new Parser<Double>(new ReadOnlySequence<Char>(test.AsMemory()));
            while (parser.Parse() is var p && p is object)
            {
                Console.WriteLine(p.ToString());
            }
        }
    }
}