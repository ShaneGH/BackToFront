using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Conditions;

namespace BackToFront.Logic.Utilities
{
    public interface IBracketedCondition<TEntity>
    {
        IAdditionalOperators<TEntity> Value(Expression<Func<TEntity, object>> value);
    }    
}
