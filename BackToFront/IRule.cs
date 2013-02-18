using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Base;

namespace BackToFront
{
    public interface IValidate
    {
        IViolation Validate(object subject);
        IEnumerable<IViolation> ValidateAll(object subject);
    }

    public interface IValidatablePathElement<TEntity> : IPathElement
    {
        IViolation Validate(TEntity subject);
        void ValidateAll(TEntity subject, IList<IViolation> violationList);
    }

    public interface IRule<TEntity, TViolation>
    {
        IOperators<TEntity, TViolation> If(Func<TEntity, object> property);
    }

    public interface IViolation 
    {
        string UserMessage { get; }
    }
}
