using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils.Expressions;

namespace BackToFront
{
    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    internal interface IValidate
    {
        /// <summary>
        /// Validate the suject and return first business rule violation
        /// </summary>
        /// <param name="subject">To validate</param>
        /// <returns>Validation or null</returns>
        IViolation ValidateEntity(object subject);

        /// <summary>
        /// Validate the subject and return all business rule violations
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <returns>Violations</returns>
        IEnumerable<IViolation> FullyValidateEntity(object subject);
    }

    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to validate</typeparam>
    internal interface IValidate<TEntity>
    {
        /// <summary>
        /// Validate the suject and return first business rule violation
        /// </summary>
        /// <param name="subject">To validate</param>
        /// <returns>Validation or null</returns>
        IViolation ValidateEntity(TEntity subject);

        /// <summary>
        /// Validate the subject and return all business rule violations
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <param name="violationList">Violations</param>
        void FullyValidateEntity(TEntity subject, IList<IViolation> violationList);
    }
}
