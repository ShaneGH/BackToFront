using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Framework
{
    public class Operator<TEntity> : RequireOperator<TEntity>, IConditionSatisfied<TEntity>
    {
        public Operator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            yield return _RequirementFailed;
            foreach (var element in base.NextPathElements(subject, context))
            {
                yield return element;
            }
        }

        public bool ConditionIsTrue(TEntity subject, Utils.Mocks mocks)
        {
            return Compile(mocks).Invoke(subject, mocks.AsValueArray);
        }

        RequirementFailed<TEntity> _RequirementFailed;
        public IModelViolation<TEntity> RequirementFailed
        {
            get
            {
                return Do(() => _RequirementFailed = new RequirementFailed<TEntity>(a => false, ParentRule));
            }
        }
    }
}
