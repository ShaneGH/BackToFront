using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils.Expressions
{
    internal class MemberExpressionWrapper : ExpressionWrapperBase<MemberExpression>
    {
        private ExpressionWrapperBase _InnerExpression;
        private ExpressionWrapperBase InnerExpression
        {
            get
            {
                return _InnerExpression ?? (_InnerExpression = CreateChildWrapper(Expression.Expression));
            }
        }

        public MemberExpressionWrapper(MemberExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as MemberExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Member == Expression.Member && InnerExpression.IsSameExpression(ex.InnerExpression);
        }

        // TODO, what if member is event or other memberinfo
        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks)
        {
            var eval = InnerExpression.Evaluate(paramaters, mocks);
            return Expression.Member.Get(eval);
        }
    }
}
