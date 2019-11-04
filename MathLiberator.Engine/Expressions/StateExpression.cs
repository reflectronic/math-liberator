using System;
using System.Linq.Expressions;
using MathLiberator.Engine.Parsing;

namespace MathLiberator.Engine.Expressions
{
    public class StateExpression<TNumber> : MLExpression<TNumber> 
        where TNumber : unmanaged
    {
        public ReadOnlyMemory<Char> Name { get; }
        public Expression<TNumber> InitialValue { get; }
        public Boolean Constant { get; }

        public StateExpression(ReadOnlyMemory<Char> name, Expression<TNumber> initialValue, Boolean constant)
        {
            Name = name;
            InitialValue = initialValue;
            Constant = constant;
        }
    }
}