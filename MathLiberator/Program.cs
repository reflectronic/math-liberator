using System;
using System.Buffers;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
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
            while (true)
            {
                lxr.Lex();
                ref var token = ref lxr.Current;
                Console.WriteLine(token.ToString());
                if (token.TokenType is TokenType.EOF) break;
            }
        }
    }
}