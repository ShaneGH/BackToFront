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
    public interface IAdditionalRuleCondition<TEntity>
    {
        ISmartConditionSatisfied<TEntity> ElseIf(Expression<Func<TEntity, bool>> property);
        ISmartConditionSatisfied<TEntity> Else { get; }
    }

    public interface IRule<TEntity>
    {
        ISmartConditionSatisfied<TEntity> If(Expression<Func<TEntity, bool>> property);
        IModelViolation2<TEntity> RequireThat(Expression<Func<TEntity, bool>> property);
    }
}
