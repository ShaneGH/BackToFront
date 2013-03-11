using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        protected override IEnumerable<ExpressionElement<TEntity>> NextPathElements(TEntity subject)
        {
                yield return Violation;
        }

        public RequirementFailed(Expression<Func<TEntity, object>> property, Rule<TEntity> rule, RequireOperators<TEntity> operators)
            : base(property, rule)
        {
            ParentIf = operators;
        }

        public IRequireOperators<TEntity> And(Expression<Func<TEntity, object>> value)
        {
            return ParentIf.AddIf(value);
        }

        public IRequireOperators<TEntity> Or(Expression<Func<TEntity, object>> value)
        {
            return ParentIf.OrIf(value);
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            violation = ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }
    }
}
