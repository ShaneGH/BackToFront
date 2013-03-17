using BackToFront.Framework;
using BackToFront.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace BackToFront
{
    /// <summary>
    /// Application business rules
    /// </summary>
    public class Rules<TEntity>
    {
        #region Static

        public static readonly Rules<TEntity> Repository = new Rules<TEntity>();
        private static readonly ProxyGenerator Gen = new ProxyGenerator();

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
        public static void Add<THelper>(Action<IRule<TEntity>, THelper> rule)
            where THelper : class
        {
            Repository._Add<THelper>(rule);
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

        private void _Add<THelper>(Action<IRule<TEntity>, THelper> rule)
            where THelper : class
        {
            var ruleObject = new Rule<TEntity>();
            var helper = typeof(THelper).IsInterface ? Gen.CreateInterfaceProxyWithoutTarget<THelper>() : Gen.CreateClassProxy<THelper>();
            ruleObject.HelperPointers.Add(new Tuple<Type,object>(typeof(THelper), helper));
            
            // apply logic to rule
            rule(ruleObject, helper);
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