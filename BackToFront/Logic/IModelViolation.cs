
using System;
namespace BackToFront.Logic
{
    public interface IModelViolation<TEntity>
    {
        IAdditionalRuleCondition<TEntity> WithModelViolation(Func<IViolation> violation);

        // TODO: uncomment
        IAdditionalRuleCondition<TEntity> WithModelViolation(string violation);
        /*IAdditionalRuleCondition<TEntity> WithModelViolation(Func<TEntity, IViolation> violation);*/
    }    
}
