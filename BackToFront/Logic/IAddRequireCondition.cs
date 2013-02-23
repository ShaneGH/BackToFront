using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IAddRequireCondition<TEntity>
    {
        IRequireOperators<TEntity> And(Expression<Func<TEntity, object>> value);

        IRequireOperators<TEntity> Or(Expression<Func<TEntity, object>> value);
    }
}
