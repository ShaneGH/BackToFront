using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BackToFront.UnitTests.Utilities
{
    public static class Extensions
    {
        public static Func<TIn, TOut> Compile<TIn, TOut>(this BackToFront.Utils.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, IEnumerable<Utils.Mock> mocks = null)
        {
            return mocks == null ? Expression.Lambda<Func<TIn, TOut>>(subject.Evaluate(), parameters).Compile()
                : Expression.Lambda<Func<TIn, TOut>>(subject.Evaluate(mocks), parameters).Compile();
        }

        public static TOut CompileAndCall<TIn, TOut>(this BackToFront.Utils.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, TIn arg, IEnumerable<Utils.Mock> mocks = null)
        {
            return Compile<TIn, TOut>(subject, parameters, mocks)(arg);
        }

        public static Func<TIn1, TIn2, TOut> Compile<TIn1, TIn2, TOut>(this BackToFront.Utils.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, IEnumerable<Utils.Mock> mocks = null)
        {
            return mocks == null ? Expression.Lambda<Func<TIn1, TIn2, TOut>>(subject.Evaluate(), parameters).Compile()
                : Expression.Lambda<Func<TIn1, TIn2, TOut>>(subject.Evaluate(mocks), parameters).Compile();
        }

        public static TOut CompileAndCall<TIn1, TIn2, TOut>(this BackToFront.Utils.Expressions.ExpressionWrapperBase subject, ReadOnlyCollection<ParameterExpression> parameters, TIn1 arg1, TIn2 arg2, IEnumerable<Utils.Mock> mocks = null)
        {
            return Compile<TIn1, TIn2, TOut>(subject, parameters, mocks)(arg1, arg2);
        }
    }
}
