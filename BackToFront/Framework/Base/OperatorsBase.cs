using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        protected OperatorsBase(Expression<Func<TEntity, object>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected abstract IConditionSatisfied<TEntity> CompileCondition(Expression<Func<TEntity, object>> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator);
        
        #region IOperators

        public IConditionSatisfied<TEntity> IsEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.Eq);
        }

        public IConditionSatisfied<TEntity> IsNotEqualTo(Expression<Func<TEntity, object>> value)
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

        public IConditionSatisfied<TEntity> GreaterThan(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.Gr);
        }

        public IConditionSatisfied<TEntity> GreaterThan(IComparable value)
        {
            return CompileCondition(a => value, Operators.Gr);
        }

        public IConditionSatisfied<TEntity> LessThan(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.Le);
        }

        public IConditionSatisfied<TEntity> LessThan(IComparable value)
        {
            return CompileCondition(a => value, Operators.Le);
        }

        public IConditionSatisfied<TEntity> GreaterThanOrEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.GrEq);
        }

        public IConditionSatisfied<TEntity> GreaterThanOrEqualTo(IComparable value)
        {
            return CompileCondition(a => value, Operators.GrEq);
        }

        public IConditionSatisfied<TEntity> LessThanOrEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.LeEq);
        }

        public IConditionSatisfied<TEntity> LessThanOrEqualTo(IComparable value)
        {
            return CompileCondition(a => value, Operators.LeEq);
        }

        public IConditionSatisfied<TEntity> IsInstanceOf(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.IsType);
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
