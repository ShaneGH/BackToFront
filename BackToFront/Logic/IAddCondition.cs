using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Conditions;
using BackToFront.Logic.Compilations;
using BackToFront.Logic.Utilities;

namespace BackToFront.Logic
{
    public interface IAddCondition<TEntity>
    {
        IOperators<TEntity> And(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> NestedAnd(Func<IBracketedCondition<TEntity>, IAdditionalCondition<TEntity>> value);

        IOperators<TEntity> Or(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> NestedOr(Func<IBracketedCondition<TEntity>, IAdditionalCondition<TEntity>> value);
    }
}
