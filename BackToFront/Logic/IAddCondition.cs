using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IAddCondition<TEntity>
    {
        IOperators<TEntity> And(Func<TEntity, object> value);

        IOperators<TEntity> Or(Func<TEntity, object> value);
    }
}
