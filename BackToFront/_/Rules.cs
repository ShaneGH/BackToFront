using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Framework;

namespace BackToFront
{
    /// <summary>
    /// Application business rules
    /// </summary>
    public class Rules
    {
        #region Static

        internal static readonly Rules Repository = new Rules();

        /// <summary>
        /// Add a business rule
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rule"></param>
        public static void Add<TEntity>(Action<IRule<TEntity>> rule)
        {
            Repository._Add(rule);
        }

        #endregion

        internal Dictionary<Type, IValidate> Registered = new Dictionary<Type, IValidate>();

        private Rules()
        { }

        /// <summary>
        /// Exposed through static method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViolation"></typeparam>
        /// <param name="rule"></param>
        private void _Add<TEntity>(Action<IRule<TEntity>> rule)
        {
            var type = typeof(TEntity);
            if (Repository.Registered.ContainsKey(type))
                throw new InvalidOperationException();

            Registered.Add(type, new Rule<TEntity>());
            rule((Rule<TEntity>)Repository.Registered[type]);
        }
    }
}