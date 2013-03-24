using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private readonly IViolation _violation;
        public ThrowViolation(IViolation violation, Rule<TEntity> parentRule)
            : base(parentRule)
        {
            if(violation == null)
                throw new ArgumentNullException("##6");

            _violation = violation;
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, Utils.Mocks mocks)
        {
            yield break;
        }

        public override IViolation ValidateEntity(TEntity subject, Utils.Mocks mocks)
        {
            if (_violation is IViolation<TEntity>)
                (_violation as IViolation<TEntity>).OnViolation(subject);

            return _violation;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            var violation = ValidateEntity(subject, mocks);
            if (violation != null)
                violationList.Add(violation);
        }
    }
}
