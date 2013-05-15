
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Meta;
using BackToFront.Utilities;

namespace BackToFront.Expressions
{
    public class InvocationExpressionWrapper : ExpressionWrapperBase<InvocationExpression>
    {
        private ExpressionWrapperBase _InnerExpression;
        public ExpressionWrapperBase InnerExpression
        {
            get
            {
                return _InnerExpression ?? (_InnerExpression = CreateChildWrapper(Expression.Expression));
            }
        }

        private ExpressionWrapperBase[] _Arguments;
        public ExpressionWrapperBase[] Arguments
        {
            get
            {
                return _Arguments ?? (_Arguments = Expression.Arguments.Select(a => CreateChildWrapper(a)).ToArray());
            }
        }

        public InvocationExpressionWrapper(InvocationExpression expression)
            : base(expression)
        {

        }

        public override bool IsSameExpression(InvocationExpression expression)
        {
            return Arguments.Length == expression.Arguments.Count &&
                Arguments.All((a, i) => a.IsSameExpression(expression.Arguments[i])) &&
                InnerExpression.IsSameExpression(expression.Expression);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return new[] { InnerExpression.GetMembersForParameter(parameter) }
                .Concat(Arguments.Select(a => a.GetMembersForParameter(parameter)))
                .Aggregate();
        }

        public ExpressionMeta _Meta;
        public override ExpressionMeta Meta
        {
            get 
            {
                return _Meta ?? (_Meta = new InvocationExpressionMeta(this));
            }
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get
            {
                return InnerExpression.UnorderedParameters.Union(Arguments.Select(a => a.UnorderedParameters).Aggregate());
            }
        }
    }
}