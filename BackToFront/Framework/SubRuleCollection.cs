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
    public class SubRuleCollection<TEntity> : PathElement<TEntity>, IRule<TEntity>
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

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            yield break;
        }

        Logic.Compilations.IConditionSatisfied<TEntity> IRule<TEntity>.If(Expression<Func<TEntity, bool>> property)
        {
            throw new NotImplementedException();
        }

        public IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> property)
        {
            var subRule = new Rule<TEntity>(ParentRule);
            _subRules.AddRule(subRule);
            return subRule.RequireThat(property);
        }

        public override IViolation ValidateEntity(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            return _subRules.ValidateEntity(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            violationList.AddRange(_subRules.FullyValidateEntity(subject, mocks));
        }
    }
}
