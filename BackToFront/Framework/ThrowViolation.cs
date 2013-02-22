using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private readonly IViolation _violation;
        public ThrowViolation(IViolation violation, Rule<TEntity> parentRule)
            : base(PathElement<TEntity>.IgnorePointer, parentRule)
        {
            if(violation == null)
                throw new ArgumentNullException("##");

            _violation = violation;
        }

        public override IViolation ValidateEntity(TEntity subject)
        {
            return _violation;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            violationList.Add(_violation);
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements
        {
            get { yield break; }
        }
    }
}
