using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Base;
using BackToFront.Framework.Base;
using BackToFront.Enum;

namespace BackToFront.Framework
{
    /// <summary>
    /// Represents the root of an If statement
    /// </summary>
    /// <typeparam name="TEntity">###@@@</typeparam>
    internal partial class If<TEntity>
    {
        /// <summary>
        /// Add an "&& condition" to this If
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>Next step</returns>
        public IOperators<TEntity> AddIf(Func<TEntity, object> property)
        {
            return new PassthroughIf(property, LogicalOperator.And, this);
        }

        /// <summary>
        /// Add an "|| condition" to this If
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>Next step</returns>
        public IOperators<TEntity> OrIf(Func<TEntity, object> property)
        {
            return new PassthroughIf(property, LogicalOperator.Or, this);
        }

        /// <summary>
        /// Used for all subsequent If 
        /// </summary>
        private class PassthroughIf : IfBase<TEntity>
        {
            private readonly LogicalOperator Op;
            private readonly If<TEntity> ParentIf;

            public PassthroughIf(Func<TEntity, object> property, LogicalOperator op, If<TEntity> parentIf)
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
            protected override IOperator<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
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
            protected override IOperator<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator)
            {
                return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as IComparable));
            }

            /// <summary>
            /// Will never be called, PassthroughIf is not part of a logical path
            /// </summary>
            protected override IEnumerable<IPathElement> NextPathElement
            {
                get { yield break; }
            }
        }
    }
}
