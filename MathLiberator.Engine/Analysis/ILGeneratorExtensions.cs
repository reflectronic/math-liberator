using System;
using System.Reflection.Emit;

namespace MathLiberator.Analysis
{
    public static class ILGeneratorExtensions
    {
        public static void EmitNumber<TNumber>(this ILGenerator il, OpCode opcode, TNumber number)
        {
            if (typeof(TNumber) == typeof(Double))
            {
                il.Emit(opcode, (Double) (object) number);
            }
            
            if (typeof(TNumber) == typeof(Single))
            {
                il.Emit(opcode, (Single) (object) number);
            }

        }
    }
}