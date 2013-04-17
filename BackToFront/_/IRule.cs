using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Compilations;

namespace BackToFront
{
    public interface IRule<TEntity>
    {
        IConditionSatisfied<TEntity> If(Expression<Func<TEntity, bool>> property);
        IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> property);
    }

    public interface IAdditionalRuleCondition<TEntity>
    {
        IConditionSatisfied<TEntity> ElseIf(Expression<Func<TEntity, bool>> property);
        IConditionSatisfied<TEntity> Else { get; }
    }
}
