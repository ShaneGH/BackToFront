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
        public readonly Type EntityType;
        public readonly object Rule;
        
        public readonly PropertyInfo DependenciesProperty;
        public readonly MethodInfo ValidateEntityMethod;
        public readonly MethodInfo FullyValidateEntityMethod;

        public ParentRuleWrapper(Type entityType, object rule)
        {
            if (!typeof(TEntity).Is(entityType))
                throw new AccessViolationException("##");

            EntityType = entityType;

            if (!rule.GetType().Is(typeof(IRuleValidation<>).MakeGenericType(EntityType)))
                throw new InvalidOperationException("##");

            Rule = rule;

            DependenciesProperty = typeof(IRuleValidation<>).MakeGenericType(EntityType).GetProperty("Dependencies");
            ValidateEntityMethod = typeof(IValidate<>).MakeGenericType(EntityType).GetMethod("ValidateEntity", new[] { EntityType, typeof(ValidationContext) });
            FullyValidateEntityMethod = typeof(IValidate<>).MakeGenericType(EntityType).GetMethod("FullyValidateEntity", new[] { EntityType, typeof(IList<IViolation>), typeof(ValidationContext) });
        }

        public List<DependencyWrapper> Dependencies
        {
            get
            {
                return (List<DependencyWrapper>)DependenciesProperty.GetValue(Rule);
            }
        }

        public IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return (IViolation)ValidateEntityMethod.Invoke(Rule, new object[] { subject, context });
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            FullyValidateEntityMethod.Invoke(Rule, new object[] { subject, violationList, context });
        }
    }
}
