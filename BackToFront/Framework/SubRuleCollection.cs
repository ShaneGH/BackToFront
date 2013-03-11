﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Framework.Condition;
using BackToFront.Framework.Requirement;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    internal class SubRuleCollection<TEntity> : PathElement<TEntity>, IRule<TEntity>
    {
        private readonly RuleCollection<TEntity> _subRules = new RuleCollection<TEntity>();
        public SubRuleCollection(Rule<TEntity> rule)
            : base(rule)
        {

        }

        public void AddSubRule(Rule<TEntity> subRule)
        {
            _subRules.AddRule(subRule);
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield break;
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            violation = _subRules.ValidateEntity(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            violationList.AddRange(_subRules.FullyValidateEntity(subject));
        }

        public IOperators<TEntity> If(Expression<Func<TEntity, object>> property)
        {
            var subRule = new Rule<TEntity>(ParentRule);
            _subRules.AddRule(subRule);
            return subRule.If(property);
        }

        public IRequireOperators<TEntity> RequireThat(Expression<Func<TEntity, object>> property)
        {
            var subRule = new Rule<TEntity>(ParentRule);
            _subRules.AddRule(subRule);
            return subRule.RequireThat(property);
        }

        Logic.Compilations.ISmartConditionSatisfied<TEntity> IRule<TEntity>.SmartIf(Expression<Func<TEntity, bool>> property)
        {
            throw new NotImplementedException();
        }
    }
}