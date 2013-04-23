using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;


namespace BackToFront.Expressions
{
    public class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
    {
        public ConstantExpressionWrapper(ConstantExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as ConstantExpressionWrapper;
            if (ex == null)
                return false;

            return Expression.Value.Equals(ex.Expression.Value);                
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

        private MetaData _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new MetaData(this));
            }
        }

        [DataContract]
        private class MetaData : ExpressionElementMeta
        {
            private readonly ConstantExpressionWrapper _Owner;

            public MetaData(ConstantExpressionWrapper owner)
            {
                _Owner = owner;
            }

            public override object Descriptor
            {
                get { return null; }
            }

            public override IEnumerable<ExpressionElementMeta> Elements
            {
                get { yield break; }
            }

            public override ExpressionWrapperType ExpressionType
            {
                get { return ExpressionWrapperType.Constant; }
            }

            public override Type Type
            {
                get { return _Owner.Expression.Type; }
            }

            public override ExpressionElementMeta Base
            {
                get { return null; }
            }
        }
    }
}
