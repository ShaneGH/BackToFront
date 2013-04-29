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

namespace BackToFront.Framework
{
    /// <summary>
    /// Describes if, else if, else logic
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MultiCondition<TEntity> : PathElement<TEntity>
    {
        public readonly IList<Operator<TEntity>> If = new List<Operator<TEntity>>();

        public MultiCondition(Rule<TEntity> rule)
            : base(rule) { }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get { return If.ToArray(); }
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                return If.Select(i => i.AffectedMembers).Aggregate();
            }
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
                return _Meta ?? (_Meta = new PathElementMeta(If.Select(i => i.Meta), null, PathElementType.MultiCondition));
            }
        }

        protected override Expression _NewCompile(SwapPropVisitor visitor)
        {
            ConditionalExpression final = null;
            var possibilities = If.Select(a => 
            {
                using (visitor.WithEntityParameter(a.EntityParameter))
                {
                    return new Tuple<Expression, Expression>(visitor.Visit(a.Descriptor.WrappedExpression), a.NewCompile(visitor));
                }
            });

            foreach (var possibility in possibilities.Reverse())
            {
                if (final == null)
                    final = Expression.IfThen(possibility.Item1, possibility.Item2);
                else
                {
                    final = Expression.IfThenElse(possibility.Item1, possibility.Item2, final);
                }
            }

            return final;
        }
    }
}