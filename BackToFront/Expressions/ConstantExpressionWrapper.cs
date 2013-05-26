using BackToFront.Enum;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront.Expressions
{
    public class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
    {
        public ConstantExpressionWrapper(ConstantExpression expression)
            : base(expression)
        {
        }

        protected override bool IsSameExpression(ConstantExpression expression)
        {
            return Expression.Value.Equals(expression.Value);                
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            yield break;
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get { yield break; }
        }
    }
}
