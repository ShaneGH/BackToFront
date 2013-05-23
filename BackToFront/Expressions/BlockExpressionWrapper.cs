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
                return _ChildExpressions ?? (_ChildExpressions = Expression.Expressions.Select(a => ExpressionWrapperBase.CreateChildWrapper(a)).ToArray());
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

        private BlockExpressionMeta _Meta;
        public override ExpressionMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new BlockExpressionMeta(this));
            }
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return ChildExpressions.Select(a => a.UnorderedParameters).Aggregate();
            }
        }

        protected override bool IsSameExpression(BlockExpression expression)
        {
            return expression.Expressions.Count == ChildExpressions.Count() && 
                ChildExpressions.All((e, i) => e.IsSameExpression(expression.Expressions[i]));;
        }
    }
}
