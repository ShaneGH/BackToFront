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

namespace BackToFront.Framework.Requirement
{
    /// <summary>
    /// Represents the root of an If statement
    /// </summary>
    /// <typeparam name="TEntity">###@@@</typeparam>
    internal partial class RequireOperators<TEntity>
    {
        /// <summary>
        /// Add an "&& condition" to this If
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>Next step</returns>
        public IRequireOperators<TEntity> AddIf(Expression<Func<TEntity, object>> property)
        {
            return new AdditionalRequireOperators(property, LogicalOperator.And, this);
        }

        /// <summary>
        /// Add an "|| condition" to this If
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>Next step</returns>
        public IRequireOperators<TEntity> OrIf(Expression<Func<TEntity, object>> property)
        {
            return new AdditionalRequireOperators(property, LogicalOperator.Or, this);
        }

        /// <summary>
        /// Used for all subsequent If 
        /// </summary>
        private class AdditionalRequireOperators : RequireOperatorsBase<TEntity>
        {
            private readonly LogicalOperator Op;
            private readonly RequireOperators<TEntity> ParentIf;

            public AdditionalRequireOperators(Expression<Func<TEntity, object>> property, LogicalOperator op, RequireOperators<TEntity> parentIf)
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
            protected override IRequirementFailed<TEntity> CompileCondition(Expression<Func<TEntity, object>> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
            {
                return Do(() =>
                {
                    ParentIf.Condition.Add(Op, Descriptor, @operator, value.Compile());
                    return ParentIf._rightHandSide;
                });
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
