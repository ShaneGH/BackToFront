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
        IOperators<TEntity> ElseIf(Expression<Func<TEntity, object>> property);
        IConditionSatisfied<TEntity> Else { get; }

        ISmartConditionSatisfied<TEntity> SmartElseIf(Expression<Func<TEntity, bool>> property);
        ISmartConditionSatisfied<TEntity> SmartElse { get; }

    }

    public interface IRule<TEntity>
    {
        IOperators<TEntity> If(Expression<Func<TEntity, object>> property);

        ISmartConditionSatisfied<TEntity> SmartIf(Expression<Func<TEntity, bool>> property);

        IRequireOperators<TEntity> RequireThat(Expression<Func<TEntity, object>> property);
    }
}
