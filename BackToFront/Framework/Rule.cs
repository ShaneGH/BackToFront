using BackToFront.Dependency;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Validation;

namespace BackToFront.Framework
{
    public class Rule<TEntity> : PathElement<TEntity>, IAdditionalRuleCondition<TEntity>, IRule<TEntity>, IValidate<TEntity>, IValidate, IRuleValidation<TEntity>, INonGenericRule
    {
        private readonly HashSet<IValidate<TEntity>> RegisteredItems = new HashSet<IValidate<TEntity>>();
        public readonly List<DependencyWrapper> _Dependencies = new List<DependencyWrapper>();
        private readonly HashSet<Rule<TEntity>> SubRules = new HashSet<Rule<TEntity>>();

        public IEnumerable<Rule<TEntity>> AllAncestorRules
        {
            get
            {
                return SubRules.Select(sr => new[] { sr }.Concat(sr.AllAncestorRules)).Aggregate();
            }
        }

        public Rule()
            : this(null)
        { }

        public Rule(Rule<TEntity> parentRule)
            : base(parentRule)
        {
            if (ParentRule != null)
                ParentRule.SubRules.Add(this);
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            // TODO: cache???
            get { return RegisteredItems.Select(a => a.AffectedMembers).Aggregate(); }
        }

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

        public void Register(IValidate<TEntity> element)
        {
            RegisteredItems.Add(element);
        }

        IViolation IValidate.ValidateEntity(object subject, Utilities.Mocks mocks)
        {
            if (subject is TEntity)
            {
                return ValidateEntity((TEntity)subject, new ValidationContext { Mocks = mocks });
            }

            throw new InvalidOperationException("##");
        }

        IEnumerable<IViolation> IValidate.FullyValidateEntity(object subject, Utilities.Mocks mocks)
        {
            if (subject is TEntity)
            {
                List<IViolation> violations = new List<IViolation>();
                FullyValidateEntity((TEntity)subject, violations, new ValidationContext { Mocks = mocks });
                return violations.ToArray();
            }

            throw new InvalidOperationException("##");
        }

        public List<DependencyWrapper> Dependencies
        {
            get { return _Dependencies; }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }
    }
}
