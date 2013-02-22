using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Framework.Base;

namespace BackToFront.Framework.Base
{
    internal abstract class OperatorsBase<TEntity> : PathElement<TEntity>, IOperators<TEntity>
    {
        protected OperatorsBase(Func<TEntity, object> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected abstract IConditionSatisfied<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator);
        protected abstract IConditionSatisfied<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator);
        protected abstract IConditionSatisfied<TEntity> CompileTypeCondition(Func<TEntity, Type> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, Type>, bool> @operator);
        
        #region IOperators

        public IConditionSatisfied<TEntity> IsEqualTo(Func<TEntity, object> value)
        {
            return CompileCondition(value, Operators.Eq);
        }

        public IConditionSatisfied<TEntity> IsNotEqualTo(Func<TEntity, object> value)
        {
            return CompileCondition(value, Operators.NEq);
        }

        public IConditionSatisfied<TEntity> IsEqualTo(object value)
        {
            return CompileCondition(a => value, Operators.Eq);
        }

        public IConditionSatisfied<TEntity> IsNotEqualTo(object value)
        {
            return CompileCondition(a => value, Operators.NEq);
        }

        public IConditionSatisfied<TEntity> IsTrue()
        {
            return CompileCondition(a => true, Operators.Eq);
        }

        public IConditionSatisfied<TEntity> IsFalse()
        {
            return CompileCondition(a => false, Operators.Eq);
        }

        public IConditionSatisfied<TEntity> IsNull()
        {
            return CompileCondition(a => null, Operators.Eq);
        }

        public IConditionSatisfied<TEntity> IsNotNull()
        {
            return CompileCondition(a => null, Operators.NEq);
        }

        public IConditionSatisfied<TEntity> GreaterThan(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, Operators.Gr);
        }

        public IConditionSatisfied<TEntity> GreaterThan(IComparable value)
        {
            return CompileIComparableCondition(a => value, Operators.Gr);
        }

        public IConditionSatisfied<TEntity> LessThan(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, Operators.Le);
        }

        public IConditionSatisfied<TEntity> LessThan(IComparable value)
        {
            return CompileIComparableCondition(a => value, Operators.Le);
        }

        public IConditionSatisfied<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, Operators.GrEq);
        }

        public IConditionSatisfied<TEntity> GreaterThanOrEqualTo(IComparable value)
        {
            return CompileIComparableCondition(a => value, Operators.GrEq);
        }

        public IConditionSatisfied<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, Operators.LeEq);
        }

        public IConditionSatisfied<TEntity> LessThanOrEqualTo(IComparable value)
        {
            return CompileIComparableCondition(a => value, Operators.LeEq);
        }

        public IConditionSatisfied<TEntity> IsInstanceOf(Func<TEntity, Type> value)
        {
            return CompileTypeCondition(value, Operators.IsType);
        }

        public IConditionSatisfied<TEntity> IsInstanceOf(Type value)
        {
            return IsInstanceOf(a => value);
        }

        public IConditionSatisfied<TEntity> IsInstanceOf<T>()
        {
            return IsInstanceOf(a => typeof(T));
        }

        #endregion
    }
}
