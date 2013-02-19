using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Base;
using BackToFront.Framework.Base;
using BackToFront.Enum;
using BackToFront.Utils;

namespace BackToFront.Framework
{
    internal partial class Operators<TEntity> : OperatorsBase<TEntity>
    {
        private readonly Condition<TEntity> Condition = new Condition<TEntity>();
        private ModelViolation1<TEntity> _rightHandSide;

        protected override IEnumerable<IPathElement<TEntity>> NextPathElements
        {
            get { yield return _rightHandSide; }
        }

        public Operators(Func<TEntity, object> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        #region IValidatablePathElement

        public override IViolation ValidateEntity(TEntity subject)
        {
            return Condition.CompiledCondition(subject) ? ValidateNext(subject) : null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            if (Condition.CompiledCondition(subject))
                ValidateAllNext(subject, violationList);
        }

        #endregion
        
        #region helpers

        protected override IModelViolation1<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
        {
            return Do(() =>
            {
                // logical operator is ignored for first element in list
                Condition.Add(LogicalOperator.Or, Descriptor, @operator, value);
                return _rightHandSide = new ModelViolation1<TEntity>(value, ParentRule, this);
            });
        }

        protected override IModelViolation1<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator)
        {
            return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as IComparable));
        }

        #endregion

    }
}
