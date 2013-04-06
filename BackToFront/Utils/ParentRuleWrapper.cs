using BackToFront.Dependency;
using BackToFront.Extensions.Reflection;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BackToFront.Utils
{
    public class ParentRuleWrapper<TEntity> : IRuleValidation<TEntity>
    {
        public readonly ReflectionWrapper Rule;

        public ParentRuleWrapper(Type entityType, object rule)
        {
            Rule = new ReflectionWrapper(rule);
        }

        public List<DependencyWrapper> Dependencies
        {
            get
            {
                return Rule.Property<List<DependencyWrapper>>("Dependencies");
            }
        }

        public IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return Rule.Method<IViolation, TEntity, ValidationContext>("ValidateEntity", subject, context);
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            Rule.Method<object, TEntity, IList<IViolation>, ValidationContext>("FullyValidateEntity", subject, violationList, context);
        }

        public IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                return Rule.Property<IEnumerable<AffectedMembers>>("AffectedMembers");                 
            }
        }

        public bool PropertyRequirement
        {
            get
            {
                return Rule.Property<bool>("PropertyRequirement");
            }
        }
    }
}
