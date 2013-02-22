using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Base;
using BackToFront.Logic.Compilations;
using BackToFront.Framework.Base;
using BackToFront.Enum;
using BackToFront.Utils;

namespace BackToFront.Framework.Requirement
{
    internal partial class RequireOperators<TEntity> : RequireOperatorsBase<TEntity>
    {
        private readonly Condition<TEntity> Condition = new Condition<TEntity>();
        private RequirementFailed<TEntity> _rightHandSide;

        protected override IEnumerable<PathElement<TEntity>> NextPathElements
        {
            get { yield return _rightHandSide; }
        }

        public RequireOperators(Func<TEntity, object> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        public override IViolation ValidateEntity(TEntity subject)
        {
            return !Condition.CompiledCondition(subject) ? ValidateNext(subject) : null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            if (!Condition.CompiledCondition(subject))
                ValidateAllNext(subject, violationList);
        }

        protected override IRequirementFailed<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
        {
            return Do(() =>
            {
                // logical operator is ignored for first element in list
                Condition.Add(LogicalOperator.Or, Descriptor, @operator, value);
                return _rightHandSide = new RequirementFailed<TEntity>(value, ParentRule, this);
            });
        }

        protected override IRequirementFailed<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator)
        {
            return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as IComparable));
        }

        protected override IRequirementFailed<TEntity> CompileTypeCondition(Func<TEntity, Type> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, Type>, bool> @operator)
        {
            return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as Type));
        }
    }
}
