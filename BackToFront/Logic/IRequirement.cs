using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequirement<TEntity>
    {
        IRequireOperand<TEntity> IsEqualTo(object value);
        IRequireOperand<TEntity> IsEqualTo(Func<TEntity, object> value);

        //IRequireOperand<TEntity> NotEqualTo(object value);
        //IRequireOperand<TEntity> NotEqualTo(Func<TEntity, object> value);

        //IRequireOperand<TEntity> GreaterThan(object value);
        //IRequireOperand<TEntity> GreaterThan(Func<TEntity, object> value);

        //IRequireOperand<TEntity> LessThan(object value);
        //IRequireOperand<TEntity> LessThan(Func<TEntity, object> value);

        //IRequireOperand<TEntity> GreaterThanOrEqualTo(object value);
        //IRequireOperand<TEntity> GreaterThanOrEqualTo(Func<TEntity, object> value);

        //IRequireOperand<TEntity> LessThanOrEqualTo(object value);
        //IRequireOperand<TEntity> LessThanOrEqualTo(Func<TEntity, object> value);
    }
}
