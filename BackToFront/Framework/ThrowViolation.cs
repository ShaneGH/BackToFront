using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Base;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Vio
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : IValidate<TEntity>, IPathElement
    {
        private readonly IViolation _violation;
        public ThrowViolation(IViolation violation)
        {
            if(violation == null)
                throw new ArgumentNullException();

            _violation = violation;
        }

        public IViolation Validate(TEntity subject)
        {
            return _violation;
        }

        public void ValidateAll(TEntity subject, IList<IViolation> violationList)
        {
            violationList.Add(_violation);
        }

        /// <summary>
        /// ThrowViolation is the end of the line
        /// </summary>
        public Logic.Base.IPathElement NextOption
        {
            get { return null; }
        }
    }
}
