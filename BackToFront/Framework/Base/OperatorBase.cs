using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// Base class, to be refactored out or used to differenciate Positive and negative outcomes
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViolation"></typeparam>
    internal abstract class OperatorBase<TEntity> : PathElement<TEntity>, IValidate<TEntity>
    {
        protected OperatorBase(Func<TEntity, object> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        /// <summary>
        /// Pass through, does nothing
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IViolation Validate(TEntity subject)
        {
            return ValidateNext(subject);
        }
        
        /// <summary>
        /// Pass through, does nothing
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violationList"></param>
        public void ValidateAll(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }
    }
}
