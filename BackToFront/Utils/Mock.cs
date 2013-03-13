
using System;
using System.Linq.Expressions;
using BackToFront.Utils.Expressions;

namespace BackToFront.Utils
{
    public enum MockBehavior
    {
        MockOnly,
        SetOnly,
        MockAndSet
    }

    internal class Mock
    {
        public readonly MockBehavior Behavior;
        public readonly ExpressionWrapperBase Expression;
        public readonly dynamic Value;

        public Mock(ExpressionWrapperBase expression, dynamic value, MockBehavior behavior)
        {
            Expression = expression;
            Value = value;
            Behavior = behavior;
        }

        public Mock(ExpressionWrapperBase expression, dynamic value)
        {
            // dynamic forces duplication of constructor logic

            Expression = expression;
            Value = value;
            Behavior = MockBehavior.MockOnly;
        }

        public static Mock Create<TEntity, TReturnVal>(Expression<Func<TEntity, TReturnVal>> expression, dynamic value)
        {
            return new Mock(ExpressionWrapperBase.ToWrapper(expression), value);
        }

        internal void SetValue(object root)
        {
            Expression.Set(root, Value);
        }
    }
}
