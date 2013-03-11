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
    /// Temp, to bind together old and new conditions
    /// </summary>
    internal interface CONDITION_IS_TRUE<TEntity>
    {
        bool ConditionIsTrue(TEntity subject);
    }

    /// <summary>
    /// Describes if, else if, else logic
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class MultiCondition<TEntity, TIf> : PathElement<TEntity>
        where TIf : PathElement<TEntity>, CONDITION_IS_TRUE<TEntity>
    {
        public readonly IList<TIf> If = new List<TIf>();

        public MultiCondition(Rule<TEntity> rule)
            : base(rule) { }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            foreach (var i in If)
            {
                if (i.ConditionIsTrue(subject))
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

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            violation = ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }
    }

    internal class Rule<TEntity> : PathElement<TEntity>, IAdditionalRuleCondition<TEntity>, IRule<TEntity>, IValidate<TEntity>
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
            return Do(() => _RequireThat = new RequirementFailed<TEntity>(a => property.Compile()(a), this));
        }

        public IViolation ValidateEntity(TEntity subject)
        {
            return ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield return _Condition;
            yield return _RequireThat;
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            throw new NotImplementedException();
        }

        MultiCondition<TEntity, Operator<TEntity>> _Condition;
        public IConditionSatisfied<TEntity> If(Expression<Func<TEntity, bool>> property)
        {
            return Do(() =>
            {
                _Condition = new MultiCondition<TEntity, Operator<TEntity>>(this);
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
    }
}
