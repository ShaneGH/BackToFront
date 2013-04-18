using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Utilities;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private readonly Func<IViolation> _violation;
        public ThrowViolation(Func<IViolation> violation, Rule<TEntity> parentRule)
            : base(parentRule)
        {
            if(violation == null)
                throw new ArgumentNullException("##6");

            _violation = violation;
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            yield break;
        }

        public override IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            var violation = _violation();
            violation.ViolatedEntity = subject;
            return violation;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            var violation = ValidateEntity(subject, context);
            if (violation != null)
                violationList.Add(violation);
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                yield break;
            }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }
    }
}
