using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Utilities;
using BackToFront.Validation;
using System.Reflection;
using System.Collections;

namespace BackToFront
{
    public interface IRules : IEnumerable<INonGenericRule> 
    {

    }
    
    /// <summary>
    /// Application business rules
    /// </summary>
    public class Rules<TEntity> : IRules//, IEnumerable<Rule<TEntity>>
    {
        public Rules()
            : this(null) { }

        public Rules(IDependencyResolver resolver)
        {
            _Resolver = resolver ?? new DummyDependencyResolver();
        }

        private static readonly Type _Type = typeof(TEntity);
        private readonly IDependencyResolver _Resolver;
        private readonly List<Rule<TEntity>> _Rules = new List<Rule<TEntity>>();

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

        public Type EntityType
        {
            get { return _Type; }
        }

        public IEnumerator<Rule<TEntity>> GetEnumerator()
        {
            return _Rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<INonGenericRule> IEnumerable<INonGenericRule>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}