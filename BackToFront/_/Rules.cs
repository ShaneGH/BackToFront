using BackToFront.Framework;
using BackToFront.Utils;
using System;
using System.Collections.Generic;

namespace BackToFront
{
    /// <summary>
    /// Application business rules
    /// </summary>
    public class Rules
    {
        #region Static

        public static readonly Rules Repository = new Rules();

        /// <summary>
        /// Add a business rule
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rule"></param>
        public static void Add<TEntity>(Action<IRule<TEntity>> rule)
        {
            Repository._Add<TEntity>(rule);
        }

        #endregion

        public readonly ReadonlyDictionary<Type, IValidate> Registered;
        private readonly Dictionary<Type, IValidate> _Registered = new Dictionary<Type, IValidate>();

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
        private void _Add<TEntity>(Action<IRule<TEntity>> rule)
        {
            var type = typeof(TEntity);
            if (!Repository._Registered.ContainsKey(type))
            {
                _Registered.Add(type, new RuleCollection<TEntity>());
            }

            var ruleObject = new Rule<TEntity>();

            // TODO: cast exception??
            ((RuleCollection<TEntity>)_Registered[type]).AddRule(ruleObject);

            // apply logic to rule
            rule(ruleObject);
        }
    }
}