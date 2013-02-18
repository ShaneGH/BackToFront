using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

namespace BackToFront.Framework.Base
{
    internal abstract class RuleChildElement<TEntity, TViolation> : PropertyElement<TEntity, TViolation>
        where TViolation : IViolation
    {
        protected readonly Rule<TEntity, TViolation> ParentRule;
        protected RuleChildElement(Func<TEntity, object> descriptor, Rule<TEntity, TViolation> rule)
            : base(descriptor)
        {
            ParentRule = rule;
        }
    }
}