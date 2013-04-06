using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BackToFront.Web
{
    public class WebValidateAttribute : ValidateMemberAttribute, IClientValidatable
    {
        /// <summary>
        /// Indicates whether to skip server side validation of the property. Default: false
        /// </summary>
        public bool ClientSideOnly { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ClientSideOnly)
                return ValidationResult.Success;

            return base.IsValid(value, validationContext);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
