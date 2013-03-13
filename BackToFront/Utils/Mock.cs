
using System;
using System.Linq.Expressions;
using BackToFront.Utils.Expressions;

namespace BackToFront.Utils
{
    internal class Mock
    {
        public readonly ExpressionWrapperBase Expression;
        public readonly dynamic Value;

        public Mock(ExpressionWrapperBase expression, dynamic value)
        {
            Expression = expression;
            Value = value;
        }

        public static Mock Create<TEntity, TReturnVal>(Expression<Func<TEntity, TReturnVal>> expression, dynamic value)
        {
            return new Mock(new FuncExpressionWrapper<TEntity, TReturnVal>(expression).Body, value);
        }
    }
}
