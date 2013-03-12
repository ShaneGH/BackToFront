using System;
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

        public override IViolation ValidateEntity(TEntity subject)
        {
            return _subRules.ValidateEntity(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            violationList.AddRange(_subRules.FullyValidateEntity(subject));
        }

        Logic.Compilations.IConditionSatisfied<TEntity> IRule<TEntity>.If(Expression<Func<TEntity, bool>> property)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> RequireThat(Expression<Func<TEntity, bool>> property)
        {
            var subRule = new Rule<TEntity>(ParentRule);
            _subRules.AddRule(subRule);
            return subRule.RequireThat(property);
        }
    }
}
