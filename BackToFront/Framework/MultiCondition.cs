﻿using BackToFront.Framework.Base;
using BackToFront.Framework.Condition;
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

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, IEnumerable<Utils.Mock> mocks)
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

        public override IViolation ValidateEntity(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            return ValidateNext(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            ValidateAllNext(subject, violationList, mocks);
        }
    }
}