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
    public class MultiCondition<TEntity> : PathElement<TEntity>
    {
        private readonly IList<Tuple<ExpressionWrapperBase, ParameterExpression, RequireOperator<TEntity>>> _If = new List<Tuple<ExpressionWrapperBase, ParameterExpression, RequireOperator<TEntity>>>();
        public IEnumerable<Tuple<ExpressionWrapperBase, ParameterExpression, RequireOperator<TEntity>>> If
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
            get { return _If.Select(a => a.Item3).ToArray(); }
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                // TODO: cache???
                return _If.Select(a => a.Item1.GetMembersForParameter(a.Item2).Select(m => new AffectedMembers { Member = m, Requirement = PropertyRequirement })).Aggregate();
            }
        }

        public RequireOperator<TEntity> Add(Expression<Func<TEntity, bool>> condition)
        {
            var success = new RequireOperator<TEntity>(ParentRule);

            //TODO: copy pasted from ExpressionElement, duplication of logic.

            if (condition == null)
                throw new ArgumentNullException("##");

            ReadOnlyCollection<ParameterExpression> paramaters;
            var wrapper = ExpressionWrapperBase.ToWrapper(condition, out paramaters);
            var param = paramaters.SingleOrDefault();
            _If.Add(new Tuple<ExpressionWrapperBase, ParameterExpression, RequireOperator<TEntity>>(wrapper, param, success));

            return success;
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta;// ?? (_Meta = new PathElementMeta(If.Select(i => i.Meta), null, PathElementType.MultiCondition));
            }
        }

        protected override Expression _Compile(SwapPropVisitor visitor)
        {
            Expression final = null;
            var possibilities = _If.Select(a => 
            {
                using (visitor.WithEntityParameter(a.Item2))
                    return new Tuple<Expression, Expression>(visitor.Visit(a.Item1.WrappedExpression), a.Item3.Compile(visitor));
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