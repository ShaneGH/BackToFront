using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;

namespace BackToFront.Framework
{
    public class RequireOperator<TEntity> : ExpressionElement<TEntity, bool>, IRequirementFailed<TEntity>
    {
        public RequireOperator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, Utils.Mocks mocks)
        {
            yield return _Then;
            yield return _RequireThat;
        }

        public override IViolation ValidateEntity(TEntity subject, Utils.Mocks mocks)
        {
            return ValidateNext(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            ValidateAllNext(subject, violationList, mocks);
        }

        private RequirementFailed<TEntity> _RequireThat = null;
        public IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> condition)
        {
            return Do(() => _RequireThat = new RequirementFailed<TEntity>(condition, ParentRule));
        }

        private SubRuleCollection<TEntity> _Then = null;
        public IAdditionalRuleCondition<TEntity> Then(Action<IRule<TEntity>> action)
        {
            Do(() => _Then = new SubRuleCollection<TEntity>(ParentRule));

            // run action on new sub rule
            action(_Then);

            // present rule to begin process again
            return ParentRule;
        }
    }
}
