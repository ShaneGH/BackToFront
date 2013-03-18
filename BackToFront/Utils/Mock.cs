
using BackToFront.Enum;
using BackToFront.Utils.Expressions;
using System;
using E = System.Linq.Expressions;

namespace BackToFront.Utils
{
    public class Mock
    {
        public readonly MockBehavior Behavior;
        public readonly ExpressionWrapperBase Expression;
        public readonly dynamic Value;
        public readonly E.ConstantExpression _NewExp;

        public Mock(ExpressionWrapperBase expression, dynamic value, MockBehavior behavior)
        {
            Expression = expression;
            Value = value;
            Behavior = behavior;
            _NewExp = E.Expression.Constant(value);
        }

        public Mock(ExpressionWrapperBase expression, dynamic value)
        {
            // dynamic forces duplication of constructor logic

            Expression = expression;
            Value = value;
            Behavior = MockBehavior.MockOnly;
            _NewExp = E.Expression.Constant(value);
        }

        public static Mock Create<TEntity, TReturnVal>(E.Expression<Func<TEntity, TReturnVal>> expression, dynamic value)
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

            (Expression as IPropertyChain).Set(root, Value);
        }
    }
}
