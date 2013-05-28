
using System;
using System.Linq.Expressions;
namespace BackToFront.Logic
{
    public interface IModelViolation<TEntity>
    {
        // TODO: is is possible to remove the Expression???
        IAdditionalRuleCondition<TEntity> WithModelViolation(Expression<Func<IViolation>> violation);
        IAdditionalRuleCondition<TEntity> WithModelViolation(string violation);
        IAdditionalRuleCondition<TEntity> WithModelViolation(Expression<Func<TEntity, IViolation>> violation);
    }    
}