﻿using BackToFront.Meta;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using E = System.Linq.Expressions;


namespace BackToFront.Expressions
{
    public class DefaultExpressionWrapper : ExpressionWrapperBase<DefaultExpression>
    {
        public DefaultExpressionWrapper()
            : this(E.Expression.Empty()) { }

        public DefaultExpressionWrapper(DefaultExpression expression)
            : base(expression)
        {
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            yield break;
        }

        public override ExpressionElementMeta Meta
        {
            get { throw new NotImplementedException(); }
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                yield break;
            }
        }

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            return expression is DefaultExpression;
        }
    }
}
