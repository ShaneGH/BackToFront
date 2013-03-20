using BackToFront.Framework;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackToFront
{
    /// <summary>
    /// Application business rules
    /// </summary>
    public class Rules<TEntity>
    {
        #region Static

        public static readonly Rules<TEntity> Repository = new Rules<TEntity>();

        /// <summary>
        /// Add a business rule
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rule"></param>
        public static void Add(Action<IRule<TEntity>> rule)
        {
            Repository._Add(rule);
        }

        /// <summary>
        /// Add a business rule
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rule"></param>
        public static void Add<TDependency>(Action<IRule<TEntity>, DependencyWrapper<TDependency>> rule)
            where TDependency : class
        {
            Repository._Add<TDependency>(rule);
        }

        #endregion

        private readonly IList<Rule<TEntity>> _Registered = new List<Rule<TEntity>>();
        public IEnumerable<Rule<TEntity>> Registered
        {
            get
            {
                return _Registered.ToArray();
            }
        }

        private void _Add<TDependency>(Action<IRule<TEntity>, DependencyWrapper<TDependency>> rule)
            where TDependency : class
        {
            var ruleObject = new Rule<TEntity>();

            var param = rule.Method.GetParameters();
            var mock1 = new DependencyWrapper<TDependency>(param[1].Name);
            ruleObject.Dependencies.Add(mock1);

            // apply logic to rule
            rule(ruleObject, mock1);
            _Registered.Add(ruleObject);
        }

        /// <summary>
        /// Exposed through static method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViolation"></typeparam>
        /// <param name="rule"></param>
        private void _Add(Action<IRule<TEntity>> rule)
        {
            var ruleObject = new Rule<TEntity>();

            // apply logic to rule
            rule(ruleObject);
            _Registered.Add(ruleObject);
        }
    }
}