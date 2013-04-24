using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Meta;
using BackToFront.Expressions;
using BackToFront.Enum;
using System.Runtime.Serialization;

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

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            foreach (var i in If)
            {
                if (i.ConditionIsTrue(subject, context.Mocks))
                {
                    yield return i;
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
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
    }
}