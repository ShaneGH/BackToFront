using BackToFront.Framework.Base;
using BackToFront.Framework.Requirement;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront.Framework.Condition
{
    public class Operator<TEntity> : ExpressionElement<TEntity, bool>, IConditionSatisfied<TEntity>
    {
        public Operator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            yield return _RequirementFailed;
            yield return _Then;
            yield return _RequireThat;
        }

        public override IViolation ValidateEntity(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            return ValidateNext(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            ValidateAllNext(subject, violationList, mocks);
        }

        public bool ConditionIsTrue(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            return Compile(subject, mocks)(subject);
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

        RequirementFailed<TEntity> _RequirementFailed;
        public IModelViolation<TEntity> RequirementFailed
        {
            get
            {
                return Do(() => _RequirementFailed = new RequirementFailed<TEntity>(a => false, ParentRule));
            }
        }
    }
}
