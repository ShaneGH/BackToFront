using BackToFront.Logic;

namespace BackToFront.Validate
{
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
            return new ValidateResult<TEntity>(test);
        }
    }
}