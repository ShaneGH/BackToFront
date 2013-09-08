using BackToFront.Logic;

namespace BackToFront.Validate
{
    public class ValidateOptions
    {
        /// <summary>
        /// If true, validates against all of the rules defined against all ancestors. Default, true
        /// </summary>
        public bool ValidateAgainstParentClassRules { get; set; }

        public ValidateOptions()
        {
            ValidateAgainstParentClassRules = true;
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
        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, Domain repository)
        {
            return Validate(test, repository, new ValidateOptions(), null);
        }

        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, Domain repository, object dependencyClasses)
        {
            return Validate(test, repository, new ValidateOptions(), dependencyClasses);
        }

        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, Domain repository, ValidateOptions options)
        {
            return Validate(test, repository, options, null);
        }

        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test, Domain repository, ValidateOptions options, object dependencyClasses)
        {
            return new ValidateResult<TEntity>(test, repository, options, dependencyClasses);
        }
    }
}