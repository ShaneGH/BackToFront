using BackToFront.Enum;
using BackToFront.Expressions;
using System;
using System.Linq.Expressions;
using System.Linq;
using E = System.Linq.Expressions;

using BackToFront.Extensions.Reflection;
using System.Collections.Generic;

namespace BackToFront.Utilities
{
    public class Mock<TMockValue> : Mock
    {
        public Mock(E.Expression expression, TMockValue value, MockBehavior behavior)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, behavior)
        {
        }

        public Mock(E.Expression expression, TMockValue value)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value)
        {
        }

        public Mock(ExpressionWrapperBase wrapperExpression, TMockValue value, MockBehavior behavior)
            : base(wrapperExpression, value, typeof(TMockValue), behavior)
        {
        }

        public Mock(ExpressionWrapperBase wrapperExpression, TMockValue value)
            : this(wrapperExpression, value, MockBehavior.MockAndSet)
        {
        }
    }

    public class Mock
    {
        public readonly MockBehavior Behavior;
        public readonly ExpressionWrapperBase Expression;
        public readonly Type ValueType;
        public readonly object Value;

        internal Mock(E.Expression expression, object value, Type valueType, MockBehavior behavior)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, valueType, behavior)
        {
        }

        internal Mock(E.Expression expression, object value, Type valueType)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, valueType)
        {
        }

        internal Mock(ExpressionWrapperBase wrapperExpression, object value, Type valueType, MockBehavior behavior)
        {
            Expression = wrapperExpression;
            Value = value;
            ValueType = valueType;
            Behavior = behavior;
        }

        internal Mock(ExpressionWrapperBase wrapperExpression, object value, Type valueType)
            : this(wrapperExpression, value, valueType, MockBehavior.MockAndSet)
        {
        }

        public static Mock Create<TEntity, TReturnVal>(E.Expression<Func<TEntity, TReturnVal>> expression, TReturnVal value)
        {
            return new Mock(ExpressionWrapperBase.ToWrapper(expression), value, typeof(TReturnVal));
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

        public bool TryForChild<TEntity, TChild>(Expression<Func<TEntity, TChild>> child, Expression root, out Mock forChild)
        {
            try
            {
                forChild = new Mock(Expression.ForChildExpression(child, root), Value, ValueType);
                return true;
            }
            catch(Exception)
            {
                forChild = null;
                return false;
            }
        }
    }
}
