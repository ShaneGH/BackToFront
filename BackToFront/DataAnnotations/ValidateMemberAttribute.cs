using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace BackToFront.DataAnnotations
{
    public class BTFValidationContext
    {
        public readonly Type ValidationType;

        public BTFValidationContext(ValidationContext validationContext)
        {
            ValidationType = validationContext.ObjectType;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateMemberAttribute : ValidationAttribute
    {
        public const string BackToFrontValidationContext = "BackToFront.DataAnnotations.BTFValidationContext";

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ctxt = ProcessValidationContext(validationContext);

            return base.IsValid(value, validationContext);
        }

        private static BTFValidationContext ProcessValidationContext(ValidationContext validationContext)
        {
            if (!validationContext.Items.ContainsKey(BackToFrontValidationContext))
            {
                var ctxt = new BTFValidationContext(validationContext);
                validationContext.Items.Add(BackToFrontValidationContext, ctxt);
            }

            if (validationContext.Items[BackToFrontValidationContext] is BTFValidationContext)
                return validationContext.Items[BackToFrontValidationContext] as BTFValidationContext;

            throw new InvalidOperationException("##");
        }
    }
}
