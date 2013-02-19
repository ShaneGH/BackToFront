using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Extensions
{
    public static class BusinessLogic
    {
        /// <summary>
        /// Validate this object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="test">The object</param>
        /// <returns>The first business rule violation encountered</returns>
        public static IViolation ValidateX<T>(this T test)
        {
            var rule = Rules.Repository.Registered[typeof(T)];

            return rule.Validate(test);
        }

        /// <summary>
        /// Validate this object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="test">The object</param>
        /// <returns>All violated rules</returns>
        public static IEnumerable<IViolation> ValidateAllX<T>(this T test)
        {
            var rule = Rules.Repository.Registered[typeof(T)];

            return rule.ValidateAll(test);
        }
    }
}