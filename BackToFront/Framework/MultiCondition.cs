using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Meta;
using BackToFront.Expressions;
using BackToFront.Enum;
using System.Runtime.Serialization;
using System;
using BackToFront.Expressions.Visitors;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace BackToFront.Framework
{
    /// <summary>
    /// Describes if, else if, else logic
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class MultiCondition<TEntity> : PathElement<TEntity>
    {
        private readonly IList<Condition> _If = new List<Condition>();
        public IEnumerable<Condition> If
        {
            get
            {
                return _If.ToArray();
            }
        }

        public MultiCondition(Rule<TEntity> rule)
            : base(rule) { }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get { return _If.Select(a => a.Action).ToArray(); }
        }

        public override IEnumerable<MemberChainItem> ValidationSubjects
        {
            get
            {
                return _If.Select(a => a.Descriptor.GetMembersForParameter(a.EntityParameter)).Aggregate();
            }
        }

        public override IEnumerable<MemberChainItem> RequiredForValidation
        {
            get
            {
                yield break;
            }
        }

        public RequireOperator<TEntity> Add(Expression<Func<TEntity, bool>> condition)
        {
            var _if = new Condition(condition, ParentRule);
            _If.Add(_if);
            return _if.Action;
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        protected override Expression _Compile(SwapPropVisitor visitor)
        {
            Expression final = null;
            var possibilities = _If.Select(a => 
            {
                using (visitor.WithEntityParameter(a.EntityParameter))
                    return new Tuple<Expression, Expression>(visitor.Visit(a.Descriptor.WrappedExpression), a.Action.Compile(visitor));
            });

            if (possibilities.Any())
            {
                final = Expression.IfThen(possibilities.Last().Item1, possibilities.Last().Item2);
                possibilities.Reverse().Skip(1).Each(a => final = Expression.IfThenElse(a.Item1, a.Item2, final));
            }

            return final ?? Expression.Empty();
        }
    }
}