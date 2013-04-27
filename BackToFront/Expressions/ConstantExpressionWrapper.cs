using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Dependency;


namespace BackToFront.Expressions
{
    public class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
    {
        public ConstantExpressionWrapper(ConstantExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;


            var ex = expression as ConstantExpression;
            if (ex == null)
                return false;

            return Expression.Value.Equals(ex.Value);                
        }

        protected override Expression CompileInnerExpression(Mocks mocks)
        {
            return Expression;
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
