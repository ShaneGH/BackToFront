using BackToFront.Dependency;
using BackToFront.Extensions.Reflection;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Framework.NonGeneric;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BackToFront.Utils
{
    public class ParentRuleWrapper<TEntity> : RuleWrapperBase, IRuleValidation<TEntity>
    {
        public ParentRuleWrapper(object rule)
            : base(rule) { }

        public IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return Rule.Method<IViolation, TEntity, ValidationContext>("ValidateEntity", subject, context);
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            Rule.Method<object, TEntity, IList<IViolation>, ValidationContext>("FullyValidateEntity", subject, violationList, context);
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
