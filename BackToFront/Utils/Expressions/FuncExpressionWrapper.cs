using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront.Utils.Expressions
{
    /// <summary>
    /// Private class, do not export as it exposes un wanted behavior (e.g. expressions without return types)
    /// </summary>
    abstract class FuncExpressionWrapperBase : ExpressionWrapperBase<LambdaExpression>
    {
        public readonly ExpressionWrapperBase Body;

        protected FuncExpressionWrapperBase(LambdaExpression expression)
            : base(expression)
        {
            Body = CreateChildWrapper(expression.Body);
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (expression is FuncExpressionWrapperBase)
                expression = (expression as FuncExpressionWrapperBase).Body;
            
            if (expression == null)
                return false;

            if (Body.WrappedExpressionParameters.Count != expression.WrappedExpressionParameters.Count)
                return false;

            for (int i = 0, ii = Body.WrappedExpressionParameters.Count; i < ii; i++)
                if (Body.WrappedExpressionParameters[i].Type != expression.WrappedExpressionParameters[i].Type)
                    return false;

            return Body.IsSameExpression(expression);
        }

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks)
        {
            return Body.Evaluate(paramaters, mocks);
        }
    }

    internal class FuncExpressionWrapper<TParam1, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam2, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TParam4, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TParam4, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TParam4, TParam5, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TParam4, TParam5, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TParam4, TParam5, TParam6, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TParam4, TParam5, TParam6, TParam7, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TOutput>> expression)
            : base(expression)
        {
        }
    }

    internal class FuncExpressionWrapper<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TOutput> : FuncExpressionWrapperBase
    {
        public FuncExpressionWrapper(Expression<Func<TParam1, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TOutput>> expression)
            : base(expression)
        {
        }
    }
}
