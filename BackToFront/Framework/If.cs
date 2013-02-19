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
    internal partial class If<TEntity> : IfBase<TEntity>, IValidate<TEntity>, IPathElement
    {
        private readonly Condition<TEntity> Condition = new Condition<TEntity>();
        private Operator<TEntity> _rightHandSide;

        protected override IEnumerable<IPathElement> NextPathElement
        {
            get { yield return _rightHandSide; }
        }

        public If(Func<TEntity, object> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        private class TMP
        {
            public Func<TEntity, bool> Eval { get; set; }
            public LogicalOperator Operator { get; set; }
        }

        #region IValidatablePathElement

        public IViolation Validate(TEntity subject)
        {
            return Condition.CompiledCondition(subject) ? ValidateNext(subject) : null;
        }

        public void ValidateAll(TEntity subject, IList<IViolation> violationList)
        {
            if (Condition.CompiledCondition(subject))
                ValidateAllNext(subject, violationList);
        }

        #endregion
        
        #region helpers

        protected override IOperator<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
        {
            return Do(() =>
            {
                // logical operator is ignored for first element in list
                Condition.Add(LogicalOperator.Or, Descriptor, @operator, value);
                return _rightHandSide = new Operator<TEntity>(value, ParentRule, this);
            });
        }

        protected override IOperator<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator)
        {
            return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as IComparable));
        }

        #endregion

    }
}
