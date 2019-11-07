using System;
using System.Buffers;

namespace MathLiberator.Engine.Syntax.Expressions
{
    public class StateExpressionSyntax<TNumber> : ExpressionSyntax<TNumber> 
        where TNumber : unmanaged
    {
        public ReadOnlySequence<Char> Name { get; }
        public ExpressionSyntax<TNumber> InitialValue { get; }
        public Boolean Constant { get; }

        public StateExpressionSyntax(ReadOnlySequence<Char> name, ExpressionSyntax<TNumber> initialValue, Boolean constant)
        {
            Name = name;
            InitialValue = initialValue;
            Constant = constant;
        }
        
        public override String? ToString() => $"{Name.ToString()} {(Constant ? ":=" : "=")} {InitialValue}";
    }
}