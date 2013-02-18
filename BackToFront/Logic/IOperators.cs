using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IOperators<TEntity, TViolation>
    {
        IOperator<TEntity, TViolation> IsTrue();
        IOperator<TEntity, TViolation> IsFalse();

        IOperator<TEntity, TViolation> IsNull();
        IOperator<TEntity, TViolation> IsNotNull();

        IOperator<TEntity, TViolation> IsEqualTo(Func<TEntity, object> value);
        IOperator<TEntity, TViolation> IsEqualTo(object value);

        IOperator<TEntity, TViolation> IsNotEqualTo(Func<TEntity, object> value);
        IOperator<TEntity, TViolation> IsNotEqualTo(object value);

        IOperator<TEntity, TViolation> GreaterThan(Func<TEntity, IComparable> value);
        IOperator<TEntity, TViolation> GreaterThan(IComparable value);

        IOperator<TEntity, TViolation> LessThan(Func<TEntity, IComparable> value);
        IOperator<TEntity, TViolation> LessThan(IComparable value);

        IOperator<TEntity, TViolation> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IOperator<TEntity, TViolation> GreaterThanOrEqualTo(IComparable value);

        IOperator<TEntity, TViolation> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IOperator<TEntity, TViolation> LessThanOrEqualTo(IComparable value);

        //IIfConditionSatisfied<TEntity, TViolation> IsInstanceOf(Func<TEntity, IComparable> value);
        //IIfConditionSatisfied<TEntity, TViolation> IsInstanceOf(Type value);
        //IIfConditionSatisfied<TEntity, TViolation> IsInstanceOf<T>();
    }
}
