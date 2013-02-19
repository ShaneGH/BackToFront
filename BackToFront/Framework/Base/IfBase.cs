using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Base;
using BackToFront.Framework.Base;

namespace BackToFront.Framework.Base
{
    internal abstract class IfBase<TEntity> : PathElement<TEntity>, IOperators<TEntity>
    {
        protected IfBase(Func<TEntity, object> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected abstract IOperator<TEntity> CompileCondition(Func<TEntity, object> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator);
        protected abstract IOperator<TEntity> CompileIComparableCondition(Func<TEntity, IComparable> value, Func<TEntity, Func<TEntity, object>, Func<TEntity, IComparable>, bool> @operator);

        #region static operator functions

        /// <summary>
        /// lhs.Equals(rhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool Eq(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
        {
            var val1 = lhs(subject);
            var val2 = rhs(subject);
            return (val1 == null && val2 == null) ||
                    (val1 != null && val1.Equals(val2));
        }

        /// <summary>
        /// !lhs.Equals(rhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool NEq(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
        {
            return !Eq(subject, lhs, rhs);
        }

        /// <summary>
        /// lhs > rhs (actually rhs &lt; lhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool Gr(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, IComparable> rhs)
        {
            var val = rhs(subject);
            return val != null && val.CompareTo(lhs(subject)) < 0;
        }

        /// <summary>
        /// lhs &lt; rhs (actually rhs > lhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool Le(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, IComparable> rhs)
        {
            var val = rhs(subject);
            return val != null && val.CompareTo(lhs(subject)) > 0;
        }

        /// <summary>
        /// !(lhs &lt; rhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool GrEq(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, IComparable> rhs)
        {
            return !Le(subject, lhs, rhs);
        }

        /// <summary>
        /// !(lhs > rhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool LeEq(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, IComparable> rhs)
        {
            return !Gr(subject, lhs, rhs);
        }

        /// <summary>
        /// lhs == null
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="unusedRhs"></param>
        /// <returns></returns>
        public static bool Null(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> unusedRhs)
        {
            var v1 = lhs(subject);
            return v1 == null;
        }

        #endregion

        #region Public Operators

        public IOperator<TEntity> IsEqualTo(Func<TEntity, object> value)
        {
            return CompileCondition(value, Eq);
        }

        public IOperator<TEntity> IsNotEqualTo(Func<TEntity, object> value)
        {
            return CompileCondition(value, NEq);
        }

        public IOperator<TEntity> IsEqualTo(object value)
        {
            return CompileCondition(a => value, Eq);
        }

        public IOperator<TEntity> IsNotEqualTo(object value)
        {
            return CompileCondition(a => value, NEq);
        }

        public IOperator<TEntity> IsTrue()
        {
            return CompileCondition(a => true, Eq);
        }

        public IOperator<TEntity> IsFalse()
        {
            return CompileCondition(a => false, Eq);
        }

        public IOperator<TEntity> IsNull()
        {
            return CompileCondition(a => null, Eq);
        }

        public IOperator<TEntity> IsNotNull()
        {
            return CompileCondition(a => null, NEq);
        }

        public IOperator<TEntity> GreaterThan(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, Gr);
        }

        public IOperator<TEntity> GreaterThan(IComparable value)
        {
            return CompileIComparableCondition(a => value, Gr);
        }

        public IOperator<TEntity> LessThan(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, Le);
        }

        public IOperator<TEntity> LessThan(IComparable value)
        {
            return CompileIComparableCondition(a => value, Le);
        }

        public IOperator<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, GrEq);
        }

        public IOperator<TEntity> GreaterThanOrEqualTo(IComparable value)
        {
            return CompileIComparableCondition(a => value, GrEq);
        }

        public IOperator<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value)
        {
            return CompileIComparableCondition(value, LeEq);
        }

        public IOperator<TEntity> LessThanOrEqualTo(IComparable value)
        {
            return CompileIComparableCondition(a => value, LeEq);
        }

        #endregion
    }
}
