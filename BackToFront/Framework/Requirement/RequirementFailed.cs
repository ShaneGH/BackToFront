using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;

using BackToFront.Logic.Compilations;

namespace BackToFront.Framework.Requirement
{
    internal class RequirementFailed<TEntity> : ModelViolation<TEntity>, IRequirementFailed<TEntity>
    {
        private readonly RequireOperators<TEntity> ParentIf;

        protected override IEnumerable<PathElement<TEntity>> NextPathElements
        {
            get
            {
                yield return Violation;
            }
        }

        public RequirementFailed(Func<TEntity, object> property, Rule<TEntity> rule, RequireOperators<TEntity> operators)
            : base(property, rule)
        {
            ParentIf = operators;
        }

        public IRequireOperators<TEntity> And(Func<TEntity, object> value)
        {
            return ParentIf.AddIf(value);
        }

        public IRequireOperators<TEntity> Or(Func<TEntity, object> value)
        {
            return ParentIf.OrIf(value);
        }

        public override IViolation ValidateEntity(TEntity subject)
        {
            return ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }
    }
}
