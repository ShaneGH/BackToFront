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
    public class BlockExpressionWrapper : ExpressionWrapperBase<BlockExpression>
    {
        private ExpressionWrapperBase[] _ChildExpressions;
        public ExpressionWrapperBase[] ChildExpressions
        {
            get
            {
                return CreateOrReference(Expression.Expressions, ref _ChildExpressions);
            }
        }

        public BlockExpressionWrapper(BlockExpression expression)
            : base(expression)
        {
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return ChildExpressions.Select(ex => ex.GetMembersForParameter(parameter)).Aggregate();
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return ChildExpressions.Select(a => a.UnorderedParameters).Aggregate();
            }
        }

        protected override bool _IsSameExpression(BlockExpression expression)
        {
            return expression.Expressions.Count == ChildExpressions.Count() && 
                ChildExpressions.All((e, i) => e.IsSameExpression(expression.Expressions[i]));;
        }
    }
}
