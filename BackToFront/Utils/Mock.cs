﻿using BackToFront.Enum;
using BackToFront.Expressions;
using System;
using System.Linq.Expressions;
using E = System.Linq.Expressions;

namespace BackToFront.Utils
{
    public class Mock
    {
        public readonly MockBehavior Behavior;
        public readonly ExpressionWrapperBase Expression;
        public readonly Type ValueType;
        public readonly object Value;

        public Mock(E.Expression expression, object value, Type valueType, MockBehavior behavior)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, valueType, behavior)
        {
        }

        public Mock(E.Expression expression, object value, Type valueType)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, valueType)
        {
        }

        public Mock(ExpressionWrapperBase wrapperExpression, object value, Type valueType, MockBehavior behavior)
        {
            Expression = wrapperExpression;
            Value = value;
            ValueType = valueType;
            Behavior = behavior;
        }

        public Mock(ExpressionWrapperBase wrapperExpression, object value, Type valueType)
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

        public Mock ForChild<TEntity, TChild>(Expression<Func<TEntity, TChild>> child)
        {
            throw new NotImplementedException();
        }

        public bool TryForChild<TEntity, TChild>(Expression<Func<TEntity, TChild>> child, out Mock forChild)
        {
            try
            {
                forChild = ForChild(child);
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
