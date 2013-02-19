using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

namespace BackToFront.Framework.Base
{
    internal abstract class RuleChildElement<TEntity> : PropertyElement<TEntity>
    {
        protected readonly Rule<TEntity> ParentRule;
        protected RuleChildElement(Func<TEntity, object> descriptor, Rule<TEntity> rule)
            : base(descriptor)
        {
            ParentRule = rule;
        }
    }
}