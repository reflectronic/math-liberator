using System;
using System.Net.Sockets;

namespace MathLiberator.Analysis
{
    public interface INumericAdapter<TNumber>
        where TNumber : unmanaged
    {
        TNumber Add(TNumber left, TNumber right);
        TNumber Subtract(TNumber left, TNumber right);
        TNumber Minus(TNumber op);
        TNumber Divide(TNumber left, TNumber right);
        TNumber Multiply(TNumber left, TNumber right);
    }

    public struct DoubleNumericAdapter : INumericAdapter<Double>
    {
        public Double Add(Double left, Double right) => left + right;

        public Double Subtract(Double left, Double right) => left - right;
        public Double Minus(Double op) => -op;
        
        public Double Divide(Double left, Double right) => left / right;

        public Double Multiply(Double left, Double right) => left * right;
    }
    
    public struct SingleNumericAdapter : INumericAdapter<Single>
    {
        public Single Add(Single left, Single right) => left + right;

        public Single Subtract(Single left, Single right) => left - right;
        
        public Single Minus(Single op) => -op;
        
        public Single Divide(Single left, Single right) => left / right;

        public Single Multiply(Single left, Single right) => left * right;
    }

}