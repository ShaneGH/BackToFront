using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Conditions;

namespace BackToFront.Logic.Utilities
{
    public interface IAdditionalCondition<TEntity>
    {
        IAdditionalOperators<TEntity> And(Func<TEntity, object> value);
        IAdditionalCondition<TEntity> NestedAnd(Func<IBracketedCondition<TEntity>, IAdditionalCondition<TEntity>> value);

        IAdditionalOperators<TEntity> Or(Func<TEntity, object> value);
        IAdditionalCondition<TEntity> NestedOr(Func<IBracketedCondition<TEntity>, IAdditionalCondition<TEntity>> value);
    }
}
