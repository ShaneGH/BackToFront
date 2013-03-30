using BackToFront.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils
{
    public class ParentRuleWrappers<TEntity> : IEnumerable<ParentRuleWrapper<TEntity>>
    {
        public readonly Type EntityType;
        private readonly IEnumerable<ParentRuleWrapper<TEntity>> InnerSomethings;

        public ParentRuleWrappers(Type entityType)
        {
            if (!typeof(TEntity).Is(entityType))
                throw new InvalidOperationException("##");

            EntityType = entityType;
            InnerSomethings = Rules.GetRules(entityType).Select(r => new ParentRuleWrapper<TEntity>(entityType, r)).ToArray();
        }

        public IEnumerator<ParentRuleWrapper<TEntity>> GetEnumerator()
        {
            return InnerSomethings.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InnerSomethings.GetEnumerator();
        }
    }
}