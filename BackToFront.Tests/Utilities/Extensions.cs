using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BackToFront.Tests.Utilities
{
    public static class Extensions
    {
        public static Func<TIn, TOut> Compile<TIn, TOut>(this BackToFront.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, Utils.Mocks mocks = null)
        {
            return mocks == null ? Expression.Lambda<Func<TIn, TOut>>(subject.Compile(), parameters).Compile()
                : Expression.Lambda<Func<TIn, TOut>>(subject.Compile(mocks), parameters).Compile();
        }

        public static TOut CompileAndCall<TIn, TOut>(this BackToFront.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, TIn arg, Utils.Mocks mocks = null)
        {
            return Compile<TIn, TOut>(subject, parameters, mocks)(arg);
        }

        public static Func<TIn1, TIn2, TOut> Compile<TIn1, TIn2, TOut>(this BackToFront.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, Utils.Mocks mocks = null)
        {
            return mocks == null ? Expression.Lambda<Func<TIn1, TIn2, TOut>>(subject.Compile(), parameters).Compile()
                : Expression.Lambda<Func<TIn1, TIn2, TOut>>(subject.Compile(mocks), parameters).Compile();
        }

        public static TOut CompileAndCall<TIn1, TIn2, TOut>(this BackToFront.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, TIn1 arg1, TIn2 arg2, Utils.Mocks mocks = null)
        {
            return Compile<TIn1, TIn2, TOut>(subject, parameters, mocks)(arg1, arg2);
        }
    }
}
