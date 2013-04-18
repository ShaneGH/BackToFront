using BackToFront.Enum;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront
{
    public interface IValidateResult
    {
        IViolation FirstViolation { get; }

        IEnumerable<IViolation> AllViolations { get; }
    }

    public interface IValidateResult<TEntity> : IValidateResult
    {
        IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value, MockBehavior behavior);
        IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value);

        /// <summary>
        /// Validate a member using the same Mocks and dependencies
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        IValidateResult<TEntity> ValidateMember<TParameter>(Expression<Func<TEntity, TParameter>> member);

        /// <summary>
        /// Validate a member using the same mocks
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="member"></param>
        /// <param name="dependencyClasses">Dependencies for the mock</param>
        /// <returns></returns>
        IValidateResult<TEntity> ValidateMember<TParameter>(Expression<Func<TEntity, TParameter>> member, object dependencyClasses);
    }

    /// <summary>
    /// Represents a business rule violation.
    /// </summary>
    public interface IViolation
    {
        /// <summary>
        /// The message to display to a user
        /// </summary>
        string UserMessage { get; }

        /// <summary>
        /// The object which is in a violated state
        /// </summary>
        object ViolatedEntity { get; set; }

        /// <summary>
        /// The properties which have been violated
        /// </summary>
        IEnumerable<MemberChainItem> Violated { get; set; }
    }
}
