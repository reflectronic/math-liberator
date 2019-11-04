using System.Collections.Generic;

namespace MathLiberator.Engine.Expressions
{
    public class ModelExpression<TNumber> : MLExpression<TNumber> 
        where TNumber : unmanaged
    {
        public MLExpression<TNumber> Start { get; }
        public MLExpression<TNumber> Step { get; }
        public MLExpression<TNumber> Condition { get; }

        public IReadOnlyList<MLExpression<TNumber>> ModelExpressions { get; }

        public ModelExpression(MLExpression<TNumber> start, MLExpression<TNumber> step, MLExpression<TNumber> condition, IReadOnlyList<MLExpression<TNumber>> modelExpressions)
        {
            Start = start;
            Step = step;
            Condition = condition;
            ModelExpressions = modelExpressions;
        }
    }
}