using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using BackToFront.Framework.Meta;

namespace BackToFront.Validation
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
        IViolation ValidateEntity(object subject, Mocks mocks);

        /// <summary>
        /// Validate the subject and return all business rule violations
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <returns>Violations</returns>
        IEnumerable<IViolation> FullyValidateEntity(object subject, Mocks mocks);
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
        IViolation ValidateEntity(TEntity subject, ValidationContext context);

        /// <summary>
        /// Validate the subject and return all business rule violations
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <param name="violationList">Violations</param>
        void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context);

        /// <summary>
        /// All of the members which are touched by this object and its children
        /// </summary>
        IEnumerable<AffectedMembers> AffectedMembers { get; }

        /// <summary>
        /// Gets the function of this element
        /// </summary>
        bool PropertyRequirement { get; }

        /// <summary>
        /// Metadata on the element to validate
        /// </summary>
        IMetaElement Meta { get; }
    }
}
