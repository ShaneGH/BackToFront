using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Framework.Base;
using BackToFront.Enum;

namespace BackToFront.Framework
{
    internal partial class If<TEntity, TViolation>
    {
        public IOperators<TEntity, TViolation> AddIf(Func<TEntity, object> property)
        {
            return new PassthroughIf(property, LogicalOperator.And, this);
        }

        public IOperators<TEntity, TViolation> OrIf(Func<TEntity, object> property)
        {
            return new PassthroughIf(property, LogicalOperator.Or, this);
        }

        private class PassthroughIf : IfBase<TEntity, TViolation>
        {
            private readonly LogicalOperator Op;
            private readonly If<TEntity, TViolation> ParentIf;

            public PassthroughIf(Func<TEntity, object> property, LogicalOperator op, If<TEntity, TViolation> parentIf)
                : base(property, parentIf.ParentRule)
            {
                ParentIf = parentIf;
                Op = op;
            }

            protected override IOperator<TEntity, TViolation> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator)
            {
                return Do(() =>
                {
                    ParentIf.Condition.Add(Op, Descriptor, @operator, value);
                    return ParentIf._rightHandSide;
                });
            }

            protected override IOperator<TEntity, TViolation> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator)
            {
                return CompileCondition(value, (a, b, c) => @operator(a, b, d => c(d) as IComparable));
            }

            /// <summary>
            /// Will never be called, PassthroughIf is not part of a logical path
            /// </summary>
            protected override IEnumerable<IValidatablePathElement<TEntity>> NextPathElement
            {
                get { yield break; }
            }
        }
    }
}
