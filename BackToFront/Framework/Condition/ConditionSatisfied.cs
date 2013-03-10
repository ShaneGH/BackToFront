using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Framework.Requirement;
using BackToFront.Logic;
using BackToFront.Logic.Utilities;

using BackToFront.Logic.Compilations;

namespace BackToFront.Framework.Condition
{
    internal class ConditionSatisfied<TEntity> : ModelViolation<TEntity>, IConditionSatisfied<TEntity>
    {
        private readonly Operators<TEntity> ParentIf;

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
                yield return Violation;
                yield return _Then;
                yield return _RequireThat;
        }

        public ConditionSatisfied(Expression<Func<TEntity, object>> property, Rule<TEntity> rule, Operators<TEntity> operators)
            : base(property, rule)
        {
            ParentIf = operators;
        }

        public IOperators<TEntity> And(Expression<Func<TEntity, object>> value)
        {
            return ParentIf.AddIf(value);
        }

        public IOperators<TEntity> Or(Expression<Func<TEntity, object>> value)
        {
            return ParentIf.OrIf(value);
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

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            violation = ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }

        public IConditionSatisfied<TEntity> NestedAnd(Func<IBracketedCondition<TEntity>, IAdditionalCondition<TEntity>> value)
        {
            throw new NotImplementedException();
        }

        public IConditionSatisfied<TEntity> NestedOr(Func<IBracketedCondition<TEntity>, IAdditionalCondition<TEntity>> value)
        {
            throw new NotImplementedException();
        }
    }
}
