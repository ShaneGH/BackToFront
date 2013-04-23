﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Utilities;
using BackToFront.Validation;

namespace BackToFront
{
    //public class RuleRepository
    //{
    //    private readonly IDependencyResolver _Resolver;
    //    private readonly Dictionary<Type, object> _Rules = new Dictionary<Type, object>();

    //    public RuleRepository()
    //        : this(new DummyDependencyResolver())
    //    {
    //    }

    //    public RuleRepository(IDependencyResolver resolver)
    //    {
    //        if (resolver == null)
    //            throw new ArgumentNullException("resolver");

    //        _Resolver = resolver;
    //    }

    //    public IEnumerable<ParentRuleWrappers<TEntity>> ParentRules<TEntity>()
    //    {
    //        var current = typeof(TEntity).BaseType;
    //        while (current != null)
    //        {
    //            yield return new ParentRuleWrappers<TEntity>(current);
    //            current = current.BaseType;
    //        }
    //    }

    //    public Rule<TEntity> Rules<TEntity>()
    //    {
    //        var t = typeof(TEntity);
    //        if (!_Rules.ContainsKey(t))
    //        {
    //            _Rules.Add(t, new Rules<TEntity>(_Resolver));
    //        }

    //        return (Rule<TEntity>)_Rules[t];
    //    }
    //}

    /// <summary>
    /// Application business rules
    /// </summary>
    public class Rules<TEntity>
    {
        #region Static

        public static readonly Rules<TEntity> Repository = new Rules<TEntity>(() => BackToFrontDependency.ProtectedResolver);

        /// <summary>
        /// The rules applied to the ancestor classes of TEntity
        /// </summary>
        public static IEnumerable<ParentRuleWrappers<TEntity>> ParentClassRepositories
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

        public Rules(Func<IDependencyResolver> resolver)
        {
            _Rules.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => { _Registered = null; };
            _Resolver = resolver;
        }

        private readonly Func<IDependencyResolver> _Resolver;
        private readonly ObservableCollection<IRuleValidation<TEntity>> _Rules = new ObservableCollection<IRuleValidation<TEntity>>();
        private IEnumerable<IRuleValidation<TEntity>> _Registered;
        public IEnumerable<IRuleValidation<TEntity>> Registered
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
            var mock1 = new DependencyWrapper<TDependency>(param[1].Name, _Resolver);
            ruleObject._Dependencies.Add(mock1);

            // apply logic to rule
            rule(ruleObject, mock1);
            _Rules.Add(ruleObject);
        }

        public void Add(Action<IRule<TEntity>> rule)
        {
            var ruleObject = new Rule<TEntity>();

            // apply logic to rule
            rule(ruleObject);
            _Rules.Add(ruleObject);
        }
    }
}