using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Base;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : IPathElement<TEntity>
    {
        private readonly IViolation _violation;
        public ThrowViolation(IViolation violation)
        {
            if(violation == null)
                throw new ArgumentNullException();

            _violation = violation;
        }

        public IViolation ValidateEntity(TEntity subject)
        {
            return _violation;
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            violationList.Add(_violation);
        }

        /// <summary>
        /// ThrowViolation is the end of the line. Returning _violation will ensure this is never called
        /// </summary>
        public Logic.Base.IPathElement<TEntity> NextOption
        {
            get { return null; }
        }
    }
}
