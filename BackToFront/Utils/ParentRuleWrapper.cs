using System.Collections.Generic;
using System.Linq;
using BackToFront.Framework;
using BackToFront.Framework.NonGeneric;

namespace BackToFront.Utils
{
    public class ParentRuleWrapper<TEntity> : RuleWrapperBase, IRuleValidation<TEntity>
    {
        public bool PropertyRequirement
        {
            get
            {
                return Rule.Property<bool>("PropertyRequirement");
            }
        }
                
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

        public IViolation ValidateEntity(object subject, Mocks mocks)
        {
            return Rule.Method<IViolation, object, Mocks>("ValidateEntity", subject, mocks);
        }

        public IEnumerable<IViolation> FullyValidateEntity(object subject, Mocks mocks)
        {
            return Rule.Method<IList<IViolation>, object, Mocks>("FullyValidateEntity", subject, mocks).ToArray();
        }
    }
}
