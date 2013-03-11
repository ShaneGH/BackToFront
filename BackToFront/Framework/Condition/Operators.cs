using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Framework.Base;
using BackToFront.Enum;
using BackToFront.Utils;

namespace BackToFront.Framework.Condition
{
    internal partial class Operators<TEntity> : OperatorsBase<TEntity>, CONDITION_IS_TRUE<TEntity>
    {
        private readonly Condition<TEntity> Condition = new Condition<TEntity>();
        private ConditionSatisfied<TEntity> _rightHandSide;

        protected override IEnumerable<ExpressionElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield return _rightHandSide;
        }

        public Operators(Expression<Func<TEntity, object>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        /// <summary>
        /// Validates the next rest of the path and returns whether the If evaluated to positive
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violation"></param>
        /// <returns>whether the If evaluated to positive</returns>
        public bool ValidateIfCondition(TEntity subject, out IViolation violation)
        {
            var output = Condition.CompiledCondition(subject);
            if (output)
                violation = ValidateNext(subject);
            else
                violation = null;

            return output;
        }

        public bool ConditionIsTrue(TEntity subject)
        {
            return Condition.CompiledCondition(subject);
        }

        /// <summary>
        /// Validates the next rest of the path and returns whether the If evaluated to positive
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violation"></param>
        /// <returns>whether the If evaluated to positive</returns>
        public bool FullyValidateIfCondition(TEntity subject, IList<IViolation> violationList)
        {
            var output = Condition.CompiledCondition(subject);
            if (output)
                ValidateAllNext(subject, violationList);

            return output;
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            ValidateIfCondition(subject, out violation);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            FullyValidateIfCondition(subject, violationList);
        }

        protected override IConditionSatisfied<TEntity> CompileCondition(Expression<Func<TEntity, object>> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
        {
            return Do(() =>
            {
                // logical operator is ignored for first element in list
                Condition.Add(LogicalOperator.Or, Descriptor, @operator, value.Compile());
                return _rightHandSide = new ConditionSatisfied<TEntity>(value, ParentRule, this);
            });
        }
    }
}
