using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework;
using BackToFront.Logic;
using BackToFront.Logic.Utilities;
using BackToFront.Utils;

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
        public static void Add<TEntity>(Action<IRuleCollection<TEntity>> rule)
        {
            Repository._Add<TEntity>(rule);
        }

        #endregion

        private readonly Dictionary<Type, IValidate> _Registered = new Dictionary<Type, IValidate>();
        internal readonly ReadonlyDictionary<Type, IValidate> Registered;

        private Rules()
        {
            Registered = new ReadonlyDictionary<Type, IValidate>(_Registered);
        }

        /// <summary>
        /// Exposed through static method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViolation"></typeparam>
        /// <param name="rule"></param>
        private void _Add<TEntity>(Action<IRuleCollection<TEntity>> rule)
        {
            var type = typeof(TEntity);
            if (!Repository._Registered.ContainsKey(type))
            {
                _Registered.Add(type, new RuleCollection<TEntity>());
            }

            // apply logic to rule
            rule((RuleCollection<TEntity>)Repository._Registered[type]);
        }
    }
}