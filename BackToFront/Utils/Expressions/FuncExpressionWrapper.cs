using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront.Utils.Expressions
{
    //TODO: more parameters
    internal class FuncExpressionWrapper<TParam1, TOutput> : ExpressionWrapperBase<Expression<Func<TParam1, TOutput>>>
    {
        public readonly ExpressionWrapperBase Body;

        public FuncExpressionWrapper(Expression<Func<TParam1, TOutput>> expression)
            : base(expression)
        {
            Body = CreateChildWrapper(expression.Body);
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (expression is FuncExpressionWrapper<TParam1, TOutput>)
                expression = (expression as FuncExpressionWrapper<TParam1, TOutput>).Body;
            
            if (expression == null)
                return false;

            if (Body.WrappedExpressionParameters.Count != expression.WrappedExpressionParameters.Count)
                return false;

            for (int i = 0, ii = Body.WrappedExpressionParameters.Count; i < ii; i++)
                if (Body.WrappedExpressionParameters[i].Type != expression.WrappedExpressionParameters[i].Type)
                    return false;

            return Body.IsSameExpression(expression);
        }

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
        {
            return Body.Evaluate(paramaters, mocks);
        }
    }

    // TODO: refactor into 1 class and unit test better
    internal class FuncExpressionWrapper<TParam1, TParam2, TOutput> : ExpressionWrapperBase<Expression<Func<TParam1, TParam2, TOutput>>>
    {
        public readonly ExpressionWrapperBase Body;

        public FuncExpressionWrapper(Expression<Func<TParam1, TParam2, TOutput>> expression)
            : base(expression)
        {
            Body = CreateChildWrapper(expression.Body);
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (expression is FuncExpressionWrapper<TParam1, TParam2, TOutput>)
                expression = (expression as FuncExpressionWrapper<TParam1, TParam2, TOutput>).Body;

            if (expression == null)
                return false;

            if (Body.WrappedExpressionParameters.Count != expression.WrappedExpressionParameters.Count)
                return false;

            for (int i = 0, ii = Body.WrappedExpressionParameters.Count; i < ii; i++)
                if (Body.WrappedExpressionParameters[i].Type != expression.WrappedExpressionParameters[i].Type)
                    return false;

            return Body.IsSameExpression(expression);
        }

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
        {
            return Body.Evaluate(paramaters, mocks);
        }
    }
}
