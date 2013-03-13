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
using BackToFront.Logic.Compilations;

namespace BackToFront.Framework
{
    /// <summary>
    /// Describes if, else if, else logic
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class MultiCondition<TEntity> : PathElement<TEntity>
    {
        public readonly IList<Operator<TEntity>> If = new List<Operator<TEntity>>();

        public MultiCondition(Rule<TEntity> rule)
            : base(rule) { }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            foreach (var i in If)
            {
                if (i.ConditionIsTrue(subject, mocks))
                {
                    yield return i;
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }

        public override IViolation ValidateEntity(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            return ValidateNext(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            ValidateAllNext(subject, violationList, mocks);
        }
    }

    internal class Rule<TEntity> : PathElement<TEntity>, IAdditionalRuleCondition<TEntity>, IRule<TEntity>
    {
        public Rule()
            : this(null)
        { }

        public Rule(Rule<TEntity> parentRule)
            : base(parentRule)
        { }

        private RequirementFailed<TEntity> _RequireThat;
        public IModelViolation2<TEntity> RequireThat(Expression<Func<TEntity, bool>> property)
        {
            return Do(() => _RequireThat = new RequirementFailed<TEntity>(property, this));
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, IEnumerable<Utils.Mock> mocks)
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
            var @if = new Operator<TEntity>(property, this);
            _Condition.If.Add(@if);
            return @if;
        }

        public IConditionSatisfied<TEntity> Else
        {
            get { return ElseIf(a => true); }
        }

        public override IViolation ValidateEntity(TEntity subject, IEnumerable<Utils.Mock> mocks)
        {
            return ValidateNext(subject, mocks);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            ValidateAllNext(subject, violationList, mocks);
        }
    }
}
