using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace BackToFront.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateMemberAttribute : ValidationAttribute
    {
        //public override bool RequiresValidationContext
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //   // validationContext.it
        //}
    }
}
