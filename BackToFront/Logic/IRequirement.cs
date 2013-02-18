using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequirement<TEntity, TViolation>
    {
        IRequireOperand<TEntity, TViolation> IsEqualTo(object value);
        IRequireOperand<TEntity, TViolation> IsEqualTo(Func<TEntity, object> value);

        //IRequireOperand<TEntity, TViolation> NotEqualTo(object value);
        //IRequireOperand<TEntity, TViolation> NotEqualTo(Func<TEntity, object> value);

        //IRequireOperand<TEntity, TViolation> GreaterThan(object value);
        //IRequireOperand<TEntity, TViolation> GreaterThan(Func<TEntity, object> value);

        //IRequireOperand<TEntity, TViolation> LessThan(object value);
        //IRequireOperand<TEntity, TViolation> LessThan(Func<TEntity, object> value);

        //IRequireOperand<TEntity, TViolation> GreaterThanOrEqualTo(object value);
        //IRequireOperand<TEntity, TViolation> GreaterThanOrEqualTo(Func<TEntity, object> value);

        //IRequireOperand<TEntity, TViolation> LessThanOrEqualTo(object value);
        //IRequireOperand<TEntity, TViolation> LessThanOrEqualTo(Func<TEntity, object> value);
    }
}
