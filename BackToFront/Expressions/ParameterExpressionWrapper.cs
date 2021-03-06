﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Extensions.Reflection;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Expressions
{
    public class ParameterExpressionWrapper : ExpressionWrapperBase<ParameterExpression>, IPropertyChainGetter
    {
        public ParameterExpressionWrapper(ParameterExpression expression)
            : base(expression)
        {
        }

        protected override bool _IsSameExpression(ParameterExpression expression)
        {
            // TODO: is this correct? Works for use case as there will only ever be one parameter in the comparing expression (the input)
            return expression.Type.Is(Expression.Type);
        }

        public object Get(object root)
        {
            if (!root.GetType().Is(Expression.Type))
            {
                throw new InvalidOperationException("##");
            }

            return root;
        }

        public ExpressionWrapperBase Root
        {
            get { return null; }
        }

        public ExpressionWrapperBase WithAlternateRoot<TEntity, TChild>(Expression root, Expression<Func<TEntity, TChild>> child)
        {
            return new ParameterExpressionWrapper(Expression);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            if (parameter == Expression)
                yield return new MemberChainItem(parameter.Type);
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get { yield return Expression; }
        }
    }
}
