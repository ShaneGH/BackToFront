using BackToFront.Utils;
using System.Collections.Generic;

namespace BackToFront
{
    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    public interface IValidate
    {
        /// <summary>
        /// Validate the suject and return first business rule violation
        /// </summary>
        /// <param name="subject">To validate</param>
        /// <returns>Validation or null</returns>
        IViolation ValidateEntity(object subject, IEnumerable<Mock> mocks);

        /// <summary>
        /// Validate the subject and return all business rule violations
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <returns>Violations</returns>
        IEnumerable<IViolation> FullyValidateEntity(object subject, IEnumerable<Mock> mocks);
    }

    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to validate</typeparam>
    public interface IValidate<TEntity>
    {
        /// <summary>
        /// Validate the suject and return first business rule violation
        /// </summary>
        /// <param name="subject">To validate</param>
        /// <returns>Validation or null</returns>
        IViolation ValidateEntity(TEntity subject, IEnumerable<Mock> mocks);

        /// <summary>
        /// Validate the subject and return all business rule violations
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <param name="violationList">Violations</param>
        void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Mock> mocks);
    }
}
