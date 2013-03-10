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
            : base(PathElement<TEntity>.IgnorePointer, rule) { }

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
            : base(PathElement<TEntity>.IgnorePointer, parentRule)
        { }

        private MultiCondition<TEntity, Operators<TEntity>> Condition;
        public IOperators<TEntity> If(Expression<Func<TEntity, object>> property)
        {
            return Do(() =>
            {
                Condition = new MultiCondition<TEntity, Operators<TEntity>>(this);
                return ElseIf(property);
            });
        }

        RequireOperators<TEntity> _require;
        public IRequireOperators<TEntity> RequireThat(Expression<Func<TEntity, object>> property)
        {
            return Do(() => _require = new RequireOperators<TEntity>(property, this));
        }

        public IOperators<TEntity> ElseIf(Expression<Func<TEntity, object>> property)
        {
            var @if = new Operators<TEntity>(property, this);
            Condition.If.Add(@if);
            return @if;
        }

        public IViolation ValidateEntity(TEntity subject)
        {
            return ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }

        public IConditionSatisfied<TEntity> Else
        {
            get { return ElseIf(a => true).IsTrue(); }
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield return _SmartCondition;
            yield return Condition;
            yield return _require;
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            throw new NotImplementedException();
        }

        MultiCondition<TEntity, SmartOperator<TEntity>> _SmartCondition;
        public ISmartConditionSatisfied<TEntity> SmartIf(Expression<Func<TEntity, bool>> property)
        {
            return Do(() =>
            {
                _SmartCondition = new MultiCondition<TEntity, SmartOperator<TEntity>>(this);
                return SmartElseIf(property);
            });
        }

        public ISmartConditionSatisfied<TEntity> SmartElseIf(Expression<Func<TEntity, bool>> property)
        {
            var @if = new SmartOperator<TEntity>(property, this);
            _SmartCondition.If.Add(@if);
            return @if;
        }

        public ISmartConditionSatisfied<TEntity> SmartElse
        {
            get { return SmartElseIf(a => true); }
        }
    }
}
