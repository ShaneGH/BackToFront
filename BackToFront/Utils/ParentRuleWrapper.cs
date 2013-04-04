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
        public readonly PropertyInfo AffectedMembersProperty;
        public readonly MethodInfo ValidateEntityMethod;
        public readonly MethodInfo FullyValidateEntityMethod;
        public readonly PropertyInfo PropertyRequirementProperty;

        public ParentRuleWrapper(Type entityType, object rule)
        {
            if (!typeof(TEntity).Is(entityType))
                throw new AccessViolationException("##");

            EntityType = entityType;

            if (!rule.GetType().Is(typeof(IRuleValidation<>).MakeGenericType(EntityType)))
                throw new InvalidOperationException("##");

            Rule = rule;

            DependenciesProperty = typeof(IRuleValidation<>).MakeGenericType(EntityType).GetProperty("Dependencies");
            AffectedMembersProperty = typeof(IValidate<>).MakeGenericType(EntityType).GetProperty("AffectedMembers");
            ValidateEntityMethod = typeof(IValidate<>).MakeGenericType(EntityType).GetMethod("ValidateEntity", new[] { EntityType, typeof(ValidationContext) });
            FullyValidateEntityMethod = typeof(IValidate<>).MakeGenericType(EntityType).GetMethod("FullyValidateEntity", new[] { EntityType, typeof(IList<IViolation>), typeof(ValidationContext) });
            PropertyRequirementProperty = typeof(IValidate<>).MakeGenericType(EntityType).GetProperty("PropertyRequirement");
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

        public IEnumerable<MemberChainItem> AffectedMembers
        {
            get
            {
                return (IEnumerable<MemberChainItem>)AffectedMembersProperty.GetValue(Rule);                    
            }
        }

        public bool PropertyRequirement
        {
            get
            {
                return (bool)PropertyRequirementProperty.GetValue(Rule);    
            }
        }
    }
}
