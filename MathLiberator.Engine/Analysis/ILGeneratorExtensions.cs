using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MathLiberator.Analysis
{
    public static class ILGeneratorExtensions
    {
        public static void EmitNumber<TNumber>(this ILGenerator il, OpCode opcode, TNumber number)
            where TNumber : unmanaged
        {
            if (typeof(TNumber) == typeof(Double))
            {
                il.Emit(opcode, Unsafe.As<TNumber, Double>(ref number));
            }

            if (typeof(TNumber) == typeof(Single))
            {
                il.Emit(opcode, Unsafe.As<TNumber, Single>(ref number));
            }
        }

        public static TNumber AsTNumber<TNumber>(this Int32 number)
            where TNumber : unmanaged
        {
            if (typeof(TNumber) == typeof(Double))
            {
                return (TNumber) (Object) (Double) number;
            }

            if (typeof(TNumber) == typeof(Single))
            {
                return (TNumber) (Object) (Single) number;
            }

            return default;
        }
    }
}