using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utils
{
    public static class Operators
    {
        /// <summary>
        /// lhs.Equals(rhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool Eq<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
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
        public static bool NEq<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
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
        public static bool Gr<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
        {
            var val = rhs(subject) as IComparable;
            return val != null && val.CompareTo(lhs(subject)) < 0;
        }

        /// <summary>
        /// lhs &lt; rhs (actually rhs > lhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool Le<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
        {
            var val = rhs(subject) as IComparable;
            return val != null && val.CompareTo(lhs(subject)) > 0;
        }

        /// <summary>
        /// !(lhs &lt; rhs)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool GrEq<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
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
        public static bool LeEq<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
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
        public static bool Null<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> unusedRhs)
        {
            var v1 = lhs(subject);
            return v1 == null;
        }

        /// <summary>
        /// lhs == null
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lhs"></param>
        /// <param name="unusedRhs"></param>
        /// <returns></returns>
        public static bool IsType<TEntity>(TEntity subject, Func<TEntity, object> lhs, Func<TEntity, object> rhs)
        {
            var v1 = lhs(subject);
            return v1 != null && v1.GetType() == rhs(subject) as Type;
        }
    }
}
