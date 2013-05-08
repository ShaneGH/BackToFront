using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;


namespace BackToFront.Expressions
{
    public class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
    {
        public ConstantExpressionWrapper(ConstantExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ConstantExpression expression)
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

        private ExpressionElementMeta _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ExpressionElementMeta(null, new ExpressionElementMeta[0], ExpressionWrapperType.Constant, Expression.Type, null));
            }
        }
    }
}
