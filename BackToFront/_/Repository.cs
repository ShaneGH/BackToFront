using BackToFront.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackToFront
{
    public class Repository
    {
        private readonly Dictionary<Type, IRules> _Rules = new Dictionary<Type, IRules>();
        private readonly IDependencyResolver _Di;

        public Repository()
            : this(null) { }

        public Repository(IDependencyResolver di)
        {
            _Di = di ?? new DummyDependencyResolver();
        }

        public Rules<TEntity> Rules<TEntity>()
        {
            return Rules(typeof(TEntity)) as Rules<TEntity>;
        }

        public bool HasRules<TEntity>()
        {
            return HasRules(typeof(TEntity));
        }

        public bool HasRules(Type entityType)
        {
            return !_Rules.ContainsKey(entityType) || !_Rules[entityType].Any();
        }

        public IRules Rules(Type entityType)
        {
            if (!_Rules.ContainsKey(entityType))
                _Rules.Add(entityType, Activator.CreateInstance(typeof(Rules<>).MakeGenericType(entityType), _Di) as IRules);

            return _Rules[entityType];
        }

        public void AddRule<TEntity>(Action<IRule<TEntity>> rule)
        {
            Rules<TEntity>().Add(rule);
        }

        public void AddRule<TEntity, TDependency>(Action<IRule<TEntity>, DependencyWrapper<TDependency>> rule)
            where TDependency : class
        {
            Rules<TEntity>().Add<TDependency>(rule);
        }
    }
}
