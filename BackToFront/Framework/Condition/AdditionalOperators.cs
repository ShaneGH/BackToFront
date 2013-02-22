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

namespace BackToFront.Framework.Condition
{
    /// <summary>
    /// Represents the root of an If statement
    /// </summary>
    /// <typeparam name="TEntity">###@@@</typeparam>
    internal partial class Operators<TEntity>
    {
        /// <summary>
        /// Add an "&& condition" to this If
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>Next step</returns>
        public IOperators<TEntity> AddIf(Func<TEntity, object> property)
        {
            return new AdditionalOperators(property, LogicalOperator.And, this);
        }

        /// <summary>
        /// Add an "|| condition" to this If
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>Next step</returns>
        public IOperators<TEntity> OrIf(Func<TEntity, object> property)
        {
            return new AdditionalOperators(property, LogicalOperator.Or, this);
        }

        /// <summary>
        /// Used for all subsequent If 
        /// </summary>
        private class AdditionalOperators : OperatorsBase<TEntity>
        {
            private readonly LogicalOperator Op;
            private readonly Operators<TEntity> ParentIf;

            public AdditionalOperators(Func<TEntity, object> property, LogicalOperator op, Operators<TEntity> parentIf)
                : base(property, parentIf.ParentRule)
            {
                ParentIf = parentIf;
                Op = op;
            }

            /// <summary>
            /// Adds condition to Parent and locks this instance
            /// </summary>
            /// <param name="value"></param>
            /// <param name="operator"></param>
            /// <returns></returns>
            protected override IConditionSatisfied<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
            {
                return Do(() =>
                {
                    ParentIf.Condition.Add(Op, Descriptor, @operator, value);
                    return ParentIf._rightHandSide;
                });
            }

            /// <summary>
            /// Adds condition to Parent and locks this instance
            /// </summary>
            /// <param name="value"></param>
            /// <param name="operator"></param>
            /// <returns></returns>
            protected override IConditionSatisfied<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator)
            {
                return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as IComparable));
            }

            protected override IConditionSatisfied<TEntity> CompileTypeCondition(Func<TEntity, Type> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, Type>, bool> @operator)
            {
                return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as Type));
            }

            #region IPathElement is not implemented as AdditionalOperators should never be part of a path

            protected override IEnumerable<PathElement<TEntity>> NextPathElements
            {
                get { yield break; }
            }

            public override IViolation ValidateEntity(TEntity subject)
            {
                return null;
            }

            public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
            {
            }

            #endregion
        }
    }
}
