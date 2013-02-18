using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Extensions
{
    public static class BusinessLogic
    {
        public static IViolation ValidateX<T>(this T test)
        {
            var rule = Rules.Repository.Registered[typeof(T)];

            return rule.Validate(test);
        }

        public static IEnumerable<IViolation> ValidateAllX<T>(this T test)
        {
            var rule = Rules.Repository.Registered[typeof(T)];

            return rule.ValidateAll(test);
        }
    }
}