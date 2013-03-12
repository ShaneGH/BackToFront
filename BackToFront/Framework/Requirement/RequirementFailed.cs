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
    internal class RequirementFailed<TEntity> : ModelViolation<TEntity, bool>, IModelViolation2<TEntity>
    {
        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield return Violation;
        }

        public RequirementFailed(Expression<Func<TEntity, bool>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            if (!(bool)Descriptor.Evaluate(new object[]{subject}))
                violation = ValidateNext(subject);
            else
                violation = null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            if (!(bool)Descriptor.Evaluate(new object[] { subject }))
                ValidateAllNext(subject, violationList);
        }
    }
}
