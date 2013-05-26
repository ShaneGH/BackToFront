using BackToFront.Meta;
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

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                yield break;
            }
        }

        protected override bool IsSameExpression(DefaultExpression expression)
        {
            return true;
        }
    }
}
