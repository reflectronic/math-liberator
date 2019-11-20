using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Lokad.ILPack;
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
            
            Console.WriteLine(test);
            Console.WriteLine();
            var lxr = new Lexer<Double>(new SequenceReader<Char>(new ReadOnlySequence<Char>(test.AsMemory())));
            Console.WriteLine("------------Lexing------------");
            while (true)
            {
                lxr.Lex();
                ref var token = ref lxr.Current;
                Console.Write(token + " ");
                if (token.Kind is SyntaxKind.EndOfFile) break;
            }
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------Parsing------------");
            var parser = new Parser<Double>(new ReadOnlySequence<Char>(test.AsMemory()));
            var compilation = parser.ParseCompilationUnit();
            Console.WriteLine(compilation);
            
            var ilgen = new ILGenerator<DoubleNumericAdapter, Double>(new DoubleNumericAdapter());
            var t = ilgen.ReturnEnumerator(compilation);

            var filename = $"{t.Name}.dll";
            new AssemblyGenerator().GenerateAssembly(t.Assembly, filename);
            
            Console.WriteLine();
            Console.WriteLine($"Wrote assembly to disk at {Path.Combine(Environment.CurrentDirectory,filename)}. Running model...");
            Console.WriteLine();
            
            typeof(Program).GetMethod(nameof(Print)).MakeGenericMethod(t).Invoke(null, null);
        }


        delegate void Printer<TStepperState>(ref TStepperState state) where TStepperState : struct, IStepper;

        public static void Print<TStepperState>()
            where TStepperState : struct, IStepper
        {
            var fields = typeof(TStepperState).GetFields().Where(f => !f.IsLiteral);
            
            var m = new DynamicMethod("Print", typeof(void), new[] { typeof(TStepperState).MakeByRefType() });
            var il = m.GetILGenerator();

            foreach (var f in fields)
            {
                il.Emit(OpCodes.Ldstr, $"{f.Name}: ");
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldflda, f);
                il.EmitCall(OpCodes.Call, f.FieldType.GetMethod(nameof(ToString), Array.Empty<Type>()), null);
                il.EmitCall(OpCodes.Call, typeof(String).GetMethod(nameof(String.Concat), new[] { typeof(String), typeof(String) }), null);
                il.EmitCall(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(String) }), null);
            }
            
            il.Emit(OpCodes.Ret);

            var del = (Printer<TStepperState>) m.CreateDelegate(typeof(Printer<TStepperState>));

            TStepperState step = default;
            step.Start();

            var i = 1;
            do
            {
                Console.WriteLine($"----------Iteration {i}----------");
                del(ref step);
                i++;
            } while (step.Step());
        }
    }
}