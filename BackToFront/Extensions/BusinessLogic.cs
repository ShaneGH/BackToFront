using BackToFront.Logic;

namespace BackToFront.Validate
{
        public class ValidateOptions
        {
            /// <summary>
            /// If true, validates against all of the rules defined against all ancestors. Default, true
            /// </summary>
            public bool ValidateAgainstParentClassRules { get; set; }

            /// <summary>
            /// If true, recursively validates fields and properties of the object, Default, false
            /// </summary>
            public bool ValidateMembers { get; set; }

            public ValidateOptions()
            {
                ValidateAgainstParentClassRules = true;
                ValidateMembers = false;
            }
        }
    public static class BusinessLogic
    {

        /// <summary>
        /// Validate this object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="test">The object</param>
        /// <returns>The first business rule violation encountered</returns>
        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test)
        {
            return Validate(test, new ValidateOptions(), null);
        }

        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, object dependencyClasses)
        {
            return Validate(test, new ValidateOptions(), dependencyClasses);
        }

        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, ValidateOptions options)
        {
            return Validate(test, options, null);
        }

        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, ValidateOptions options, object dependencyClasses)
        {
            return new ValidateResult<TEntity>(test, options, dependencyClasses);
        }
    }
}