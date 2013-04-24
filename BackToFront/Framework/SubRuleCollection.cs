using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;
using System.Runtime.Serialization;

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

        public IEnumerable<PathElement<TEntity>> NextPathElements()
        {
            yield break;
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            return NextPathElements();
        }

        public IConditionSatisfied<TEntity> If(Expression<Func<TEntity, bool>> property)
        {
            var subRule = new Rule<TEntity>(ParentRule);
            _subRules.AddRule(subRule);
            return subRule.If(property);
        }

        public IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> property)
        {
            var subRule = new Rule<TEntity>(ParentRule);
            _subRules.AddRule(subRule);
            return subRule.RequireThat(property);
        }

        public override IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return _subRules.ValidateEntity(subject, context);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            _subRules.FullyValidateEntity(subject, violationList, context);
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get 
            {
                return _subRules.AffectedMembers;
            }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(new[] { _subRules.Meta }, null, PathElementType.SubRuleCollection));
            }
        }
    }
}
