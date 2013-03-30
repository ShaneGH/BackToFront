using BackToFront.Framework;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public static IEnumerable<IEnumerable<IRuleXXX<TEntity>>> ParentClassRepositories
        {
            get 
            {
                var current = typeof(TEntity).BaseType;
                while (current != null)
                {
                    yield return new ParentRuleWrappers<TEntity>(current);
                    current = current.BaseType;
                }
            }
        }

        /// <summary>
        /// Add a business rule
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rule"></param>
        public static void AddRule(Action<IRule<TEntity>> rule)
        {
            Repository.Add(rule);
        }

        /// <summary>
        /// Add a business rule
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="rule"></param>
        public static void AddRule<TDependency>(Action<IRule<TEntity>, DependencyWrapper<TDependency>> rule)
            where TDependency : class
        {
            Repository.Add<TDependency>(rule);
        }

        #endregion

        private Rules()
        {
            _Rules.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => { _Registered = null; };
        }

        private readonly ObservableCollection<IRuleXXX<TEntity>> _Rules = new ObservableCollection<IRuleXXX<TEntity>>();
        public IEnumerable<IRuleXXX<TEntity>> _Registered;
        public IEnumerable<IRuleXXX<TEntity>> Registered
        {
            get
            {
                return _Registered ?? (_Registered = _Rules.ToArray()); 
            }
        }

        public void Add<TDependency>(Action<IRule<TEntity>, DependencyWrapper<TDependency>> rule)
            where TDependency : class
        {
            var ruleObject = new Rule<TEntity>();

            var param = rule.Method.GetParameters();
            var mock1 = new DependencyWrapper<TDependency>(param[1].Name);
            ruleObject.Dependencies.Add(mock1);

            // apply logic to rule
            rule(ruleObject, mock1);
            _Rules.Add(ruleObject);
        }

        /// <summary>
        /// Exposed through static method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViolation"></typeparam>
        /// <param name="rule"></param>
        public void Add(Action<IRule<TEntity>> rule)
        {
            var ruleObject = new Rule<TEntity>();

            // apply logic to rule
            rule(ruleObject);
            _Rules.Add(ruleObject);
        }
    }
}