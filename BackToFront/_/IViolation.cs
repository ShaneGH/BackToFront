using BackToFront.Enum;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront
{
    public interface IValidateResult<TEntity>
    {
        IViolation FirstViolation { get; }

        IEnumerable<IViolation> AllViolations { get; }

        IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value, MockBehavior behavior);
        IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value);
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
    }

    /// <summary>
    /// Represents a business rule violation.
    /// </summary>
    public interface IViolation<TEntity> : IViolation
    {
        void OnViolation(TEntity subject);
    }
}
