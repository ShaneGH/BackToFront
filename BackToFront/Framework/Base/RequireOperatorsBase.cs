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
    internal abstract class RequireOperatorsBase<TEntity> : ExpressionElement<TEntity>, IRequireOperators<TEntity>
    {
        protected RequireOperatorsBase(Expression<Func<TEntity, object>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected abstract IRequirementFailed<TEntity> CompileCondition(Expression<Func<TEntity, object>> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator);
        //protected abstract IRequirementFailed<TEntity> CompileIComparableCondition(Expression<Func<TEntity, IComparable>> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator);
        //protected abstract IRequirementFailed<TEntity> CompileTypeCondition(Expression<Func<TEntity, Type>> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, Type>, bool> @operator);

        #region IRequireOperators

        public IRequirementFailed<TEntity> IsEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.Eq);
        }

        public IRequirementFailed<TEntity> IsNotEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.NEq);
        }

        public IRequirementFailed<TEntity> IsEqualTo(object value)
        {
            return CompileCondition(a => value, Operators.Eq);
        }

        public IRequirementFailed<TEntity> IsNotEqualTo(object value)
        {
            return CompileCondition(a => value, Operators.NEq);
        }

        public IRequirementFailed<TEntity> IsTrue()
        {
            return CompileCondition(a => true, Operators.Eq);
        }

        public IRequirementFailed<TEntity> IsFalse()
        {
            return CompileCondition(a => false, Operators.Eq);
        }

        public IRequirementFailed<TEntity> IsNull()
        {
            return CompileCondition(a => null, Operators.Eq);
        }

        public IRequirementFailed<TEntity> IsNotNull()
        {
            return CompileCondition(a => null, Operators.NEq);
        }

        public IRequirementFailed<TEntity> GreaterThan(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.Gr);
        }

        public IRequirementFailed<TEntity> GreaterThan(IComparable value)
        {
            return CompileCondition(a => value, Operators.Gr);
        }

        public IRequirementFailed<TEntity> LessThan(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.Le);
        }

        public IRequirementFailed<TEntity> LessThan(IComparable value)
        {
            return CompileCondition(a => value, Operators.Le);
        }

        public IRequirementFailed<TEntity> GreaterThanOrEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.GrEq);
        }

        public IRequirementFailed<TEntity> GreaterThanOrEqualTo(IComparable value)
        {
            return CompileCondition(a => value, Operators.GrEq);
        }

        public IRequirementFailed<TEntity> LessThanOrEqualTo(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.LeEq);
        }

        public IRequirementFailed<TEntity> LessThanOrEqualTo(IComparable value)
        {
            return CompileCondition(a => value, Operators.LeEq);
        }

        public IRequirementFailed<TEntity> IsInstanceOf(Expression<Func<TEntity, object>> value)
        {
            return CompileCondition(value, Operators.IsType);
        }

        public IRequirementFailed<TEntity> IsInstanceOf(Type value)
        {
            return IsInstanceOf(a => value);
        }

        public IRequirementFailed<TEntity> IsInstanceOf<T>()
        {
            return IsInstanceOf(a => typeof(T));
        }

        #endregion

    }
}
