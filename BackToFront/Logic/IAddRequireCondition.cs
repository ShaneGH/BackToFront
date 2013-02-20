using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IAddRequireCondition<TEntity>
    {
        IRequireOperators<TEntity> And(Func<TEntity, object> value);

        IRequireOperators<TEntity> Or(Func<TEntity, object> value);
    }
}
