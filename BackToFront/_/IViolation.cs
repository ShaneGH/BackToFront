using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront
{
    public interface IValidateResult<TEntity>
    {
        IViolation FirstViolation { get; }

        IEnumerable<IViolation> AllViolations { get; }

        IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value);

        void SetAllMocks();
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
    }

    /// <summary>
    /// Represents a business rule violation.
    /// </summary>
    public interface IViolation<TEntity> : IViolation
    {
        void OnViolation(TEntity subject);
    }
}
