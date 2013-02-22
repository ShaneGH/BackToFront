using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Conditions;

namespace BackToFront.Logic.Utilities
{
    public interface IBracketedCondition<TEntity>
    {
        IAdditionalOperators<TEntity> Value(Func<TEntity, object> value);
    }    
}
