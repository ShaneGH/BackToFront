using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Framework.Meta;
using BackToFront.Expressions;
using BackToFront.Enum;

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

        private MetaData _Meta;
        public override IMetaElement Meta
        {
            get { return _Meta ?? (_Meta = new MetaData(this)); }
        }

        private class MetaData : IMetaElement
        {
            private readonly MultiCondition<TEntity> _Owner;

            public MetaData(MultiCondition<TEntity> owner)
            {
                _Owner = owner;
            }

            public IEnumerable<IMetaElement> Children
            {
                get 
                {
                    return _Owner.If.Select(i => i.Meta);
                }
            }

            public ExpressionWrapperBase Code
            {
                get { return null; }
            }

            public PathElementType Type
            {
                get { return PathElementType.MultiCondition; }
            }
        }
    }
}