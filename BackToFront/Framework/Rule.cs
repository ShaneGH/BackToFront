using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackToFront.Framework
{
    public interface IRuleValidation<TEntity> : IValidate<TEntity>
    {
        List<DependencyWrapper> Dependencies { get; }
    }

    public class Rule<TEntity> : PathElement<TEntity>, IAdditionalRuleCondition<TEntity>, IRule<TEntity>, IValidate, IRuleValidation<TEntity>
    {
        public readonly List<DependencyWrapper> Dependencies = new List<DependencyWrapper>();

        public Rule()
            : this(null)
        { }

        public Rule(Rule<TEntity> parentRule)
            : base(parentRule)
        { }

        private RequirementFailed<TEntity> _RequireThat;
        public IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> property)
        {
            return Do(() => _RequireThat = new RequirementFailed<TEntity>(property, this));
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            yield return _Condition;
            yield return _RequireThat;
        }

        MultiCondition<TEntity> _Condition;
        public IConditionSatisfied<TEntity> If(Expression<Func<TEntity, bool>> property)
        {
            return Do(() =>
            {
                _Condition = new MultiCondition<TEntity>(this);
                return ElseIf(property);
            });
        }

        public IConditionSatisfied<TEntity> ElseIf(Expression<Func<TEntity, bool>> property)
        {
            if (_Condition == null)
                return If(property);

            var @if = new Operator<TEntity>(property, this);
            _Condition.If.Add(@if);
            return @if;
        }

        public IConditionSatisfied<TEntity> Else
        {
            get { return ElseIf(a => true); }
        }

        IViolation IValidate.ValidateEntity(object subject, Utils.Mocks mocks)
        {
            if (subject is TEntity)
            {
                return ValidateEntity((TEntity)subject, new ValidationContext { Mocks = mocks });
            }

            throw new InvalidOperationException("##");
        }

        IEnumerable<IViolation> IValidate.FullyValidateEntity(object subject, Utils.Mocks mocks)
        {
            if (subject is TEntity)
            {
                List<IViolation> violations = new List<IViolation>();
                FullyValidateEntity((TEntity)subject, violations, new ValidationContext { Mocks = mocks });
                return violations.ToArray();
            }

            throw new InvalidOperationException("##");
        }

        List<DependencyWrapper> IRuleValidation<TEntity>.Dependencies
        {
            get { return this.Dependencies; }
        }
    }
}
