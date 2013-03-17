
namespace BackToFront.Logic
{
    public interface IModelViolation<TEntity>
    {
        IAdditionalRuleCondition<TEntity> OrModelViolationIs(IViolation violation);
    }    
}
