using System;
using System.Buffers;
using System.Reflection;
using MathLiberator.Analysis;
using MathLiberator.Syntax;

namespace MathLiberator
{
    static class Program
    {
        static void Main(String[] args)
        {
            var test = @"# a falling object with a mass of 10 kilograms (without air resistance)
g := 9.81
mass := 10
force := g * mass
acceleration := force / mass

velocity = 0
height = 100

[0:1:height>=0]
{
    velocity += acceleration
    height -= velocity
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
            var compilation = parser.ParseCompilationUnit();
            Console.WriteLine(compilation);
            
            Console.WriteLine("---Generating IL---");
            var ilgen = new ILGenerator<DoubleNumericAdapter, Double>(new DoubleNumericAdapter());
            var t = ilgen.ReturnEnumerator(compilation);

            foreach (var m in t.GetMembers())
            {
                Console.Write(m.ToString());

                if (m is FieldInfo { IsLiteral: true } f)
                {
                    Console.Write($" = {f.GetRawConstantValue()}");
                }
                
                Console.WriteLine();
            }
        }
    }
}