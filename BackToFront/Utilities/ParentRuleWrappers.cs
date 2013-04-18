using BackToFront.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utilities
{
    public class ParentRuleWrappers<TEntity> : IEnumerable<ParentRuleWrapper<TEntity>>
    {
        public readonly Type EntityType;
        private readonly IEnumerable<ParentRuleWrapper<TEntity>> Rules;

        public ParentRuleWrappers(Type entityType)
        {
            if (!typeof(TEntity).Is(entityType))
                throw new InvalidOperationException("##");

            EntityType = entityType;
            Rules = BackToFront.Rules.GetRules(entityType).Select(r => new ParentRuleWrapper<TEntity>(r)).ToArray();
        }

        public IEnumerator<ParentRuleWrapper<TEntity>> GetEnumerator()
        {
            return Rules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Rules.GetEnumerator();
        }
    }
}