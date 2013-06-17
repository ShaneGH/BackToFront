using System.Collections.Generic;
using System.Linq.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;

namespace BackToFront.Validation
{

    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to validate</typeparam>
    public interface IValidate<TEntity>
    {
        IEnumerable<MemberChainItem> ValidationSubjects { get; }

        IEnumerable<MemberChainItem> RequiredForValidation { get; }

        Expression Compile(SwapPropVisitor visitor);
    }
}
