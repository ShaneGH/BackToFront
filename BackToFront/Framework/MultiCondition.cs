using BackToFront.Framework.Base;
using System.Collections.Generic;

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

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, Utils.Mocks mocks)
        {
            foreach (var i in If)
            {
                if (i.ConditionIsTrue(subject, mocks))
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

        public override IViolation ValidateEntity(TEntity subject, Utils.Mocks mocks)
        {
            return ValidateNext(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            ValidateAllNext(subject, violationList, mocks);
        }
    }
}