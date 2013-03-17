using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;

using BackToFront.Logic.Compilations;

namespace BackToFront.Framework.Requirement
{
    public class RequirementFailed<TEntity> : ModelViolation<TEntity, bool>, IModelViolation<TEntity>
    {
        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            yield return Violation;
        }

        public RequirementFailed(Expression<Func<TEntity, bool>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        public override IViolation ValidateEntity(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            if (!(bool)Descriptor.Evaluate(new object[] { subject }, mocks))
                return ValidateNext(subject, mocks);
            else
                return null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            if (!(bool)Descriptor.Evaluate(new object[] { subject }, mocks))
                ValidateAllNext(subject, violationList, mocks);
        }
    }
}
