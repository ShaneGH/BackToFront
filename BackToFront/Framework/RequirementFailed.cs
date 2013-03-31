using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;

using BackToFront.Logic.Compilations;

namespace BackToFront.Framework
{
    public class RequirementFailed<TEntity> : ExpressionElement<TEntity, bool>, IModelViolation<TEntity>
    {
        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, Utils.Mocks mocks)
        {
            yield return Violation;
        }

        public RequirementFailed(Expression<Func<TEntity, bool>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected ThrowViolation<TEntity> Violation;
        public IAdditionalRuleCondition<TEntity> WithModelViolation(Func<IViolation> violation)
        {
            Do(() => { Violation = new ThrowViolation<TEntity>(violation, ParentRule); });
            return ParentRule;
        }

        public override IViolation ValidateEntity(TEntity subject, Utils.Mocks mocks)
        {
            if (!Compile(mocks).Invoke(subject, mocks.AsValueArray))
                return base.ValidateEntity(subject, mocks);
            else
                return null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            if (!Compile(mocks).Invoke(subject, mocks.AsValueArray))
                base.FullyValidateEntity(subject, violationList, mocks);
        }
    }
}
