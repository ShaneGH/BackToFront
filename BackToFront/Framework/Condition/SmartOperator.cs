using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Framework.Requirement;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;

namespace BackToFront.Framework.Condition
{
    internal class SmartOperator<TEntity> : ModelViolation<TEntity>, CONDITION_IS_TRUE<TEntity>, ISmartConditionSatisfied<TEntity>
    {
        readonly Expression<Func<TEntity, bool>> IfCodition;

        public SmartOperator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(PathElement<TEntity>.IgnorePointer, rule)
            //TODO: base(descriptor, rule)
        {
            IfCodition = descriptor;
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield return Violation;
            yield return _Then;
            yield return _RequireThat;
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            violation = ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }

        public bool ConditionIsTrue(TEntity subject)
        {
            return IfCodition.Compile()(subject);
        }

        private RequireOperators<TEntity> _RequireThat = null;
        public IRequireOperators<TEntity> RequireThat(Expression<Func<TEntity, object>> property)
        {
            return Do(() => _RequireThat = new RequireOperators<TEntity>(property, ParentRule));
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
