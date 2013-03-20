
using BackToFront.Enum;
using BackToFront.Expressions;
using System;
using E = System.Linq.Expressions;

namespace BackToFront.Utils
{
    public class Mock
    {
        public readonly MockBehavior Behavior;
        public readonly ExpressionWrapperBase Expression;
        public readonly E.ConstantExpression Value;

        public Mock(ExpressionWrapperBase expression, object value, MockBehavior behavior)
        {
            Expression = expression;
            Behavior = behavior;
            Value = E.Expression.Constant(value is BackToFront.Logic.Dependency ? (value as BackToFront.Logic.Dependency).Value : value);
        }

        public Mock(ExpressionWrapperBase expression, object value)
            : this(expression, value, MockBehavior.MockOnly)
        {
        }

        public static Mock Create<TEntity, TReturnVal>(E.Expression<Func<TEntity, TReturnVal>> expression, object value)
        {
            return new Mock(ExpressionWrapperBase.ToWrapper(expression), value);
        }

        public bool CanSet
        {
            get
            {
                return Expression is IPropertyChain && (Expression as IPropertyChain).CanSet;
            }
        }

        internal void SetValue(object root)
        {
            if (!CanSet)
                throw new InvalidOperationException("##");

            (Expression as IPropertyChain).Set(root, Value.Value);
        }
    }
}
