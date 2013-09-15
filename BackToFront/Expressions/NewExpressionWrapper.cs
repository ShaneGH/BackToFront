using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Utilities;
using System;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Expressions
{
    public class NewExpressionWrapper : ExpressionWrapperBase<NewExpression>
    {
        public bool IsAnonymous
        {
            get
            {
                return Expression.Members != null;
            }
        }

        private ExpressionWrapperBase[] _Arguments;
        public ExpressionWrapperBase[] Arguments
        {
            get
            {
                return CreateOrReference(Expression.Arguments, ref _Arguments);
            }
        }

        public NewExpressionWrapper(NewExpression expression)
            : base(expression) 
        {
        }

        protected override bool _IsSameExpression(NewExpression expression)
        {
            return this.Expression.Equals(expression) ||
                (Expression.Constructor == expression.Constructor &&
                CheckMembers(expression) &&
                Arguments.Length == expression.Arguments.Count &&
                Arguments.All((a, i) => a.IsSameExpression(expression.Arguments[i])));
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return Arguments.Select(a => a.GetMembersForParameter(parameter)).Aggregate();
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get { return Arguments.Select(a => a.UnorderedParameters).Aggregate(); }
        }

        private bool CheckMembers(NewExpression expression)
        {
            // non anonymous
            if (Expression.Members == null && expression.Members == null)
                return true;

            // one is anonymous
            if (Expression.Members == null || expression.Members == null)
                return false;

            // 
            return
                Expression.Members.Count == expression.Members.Count &&
                Expression.Members.All((a, i) => a.Equals(expression.Members[i]));
        }
    }
}