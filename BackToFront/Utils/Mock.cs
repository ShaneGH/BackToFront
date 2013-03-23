
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

        public Mock(E.Expression expression, object value, MockBehavior behavior)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, behavior)
        {
        }

        public Mock(E.Expression expression, object value)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value)
        {
        }

        public Mock(ExpressionWrapperBase wrapperExpression, object value, MockBehavior behavior)
        {
            Expression = wrapperExpression;
            Behavior = behavior;
            Value = E.Expression.Constant(value is BackToFront.Logic.Dependency ? (value as BackToFront.Logic.Dependency).Value : value);
        }

        public Mock(ExpressionWrapperBase wrapperExpression, object value)
            : this(wrapperExpression, value, MockBehavior.MockAndSet)
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
