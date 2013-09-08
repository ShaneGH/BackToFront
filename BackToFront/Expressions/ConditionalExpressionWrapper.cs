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
using System.Linq;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Expressions
{
    public class ConditionalExpressionWrapper : ExpressionWrapperBase<ConditionalExpression>
    {
        private ExpressionWrapperBase _Test;
        public ExpressionWrapperBase Test
        {
            get
            {
                return CreateOrReference(Expression.Test, ref _Test);
            }
        }

        private ExpressionWrapperBase _IfTrue;
        public ExpressionWrapperBase IfTrue
        {
            get
            {
                return CreateOrReference(Expression.IfTrue, ref _IfTrue);
            }
        }

        private ExpressionWrapperBase _IfFalse;
        public ExpressionWrapperBase IfFalse
        {
            get
            {
                return CreateOrReference(Expression.IfFalse, ref _IfFalse);
            }
        }

        public ConditionalExpressionWrapper(ConditionalExpression expression)
            : base(expression)
        {
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return Test.GetMembersForParameter(parameter)
                .Union(IfTrue.GetMembersForParameter(parameter))
                .Union(IfFalse.GetMembersForParameter(parameter));
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Test.UnorderedParameters
                    .Union(IfTrue.UnorderedParameters)
                    .Union(IfFalse.UnorderedParameters);
            }
        }

        protected override bool _IsSameExpression(ConditionalExpression expression)
        {
            return Test.IsSameExpression(expression.Test) &&
                IfTrue.IsSameExpression(expression.IfTrue) && 
                IfFalse.IsSameExpression(expression.IfFalse);
        }
    }
}
