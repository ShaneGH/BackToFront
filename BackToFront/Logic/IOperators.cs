using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IOperators<TEntity>
    {
        IOperator<TEntity> IsTrue();
        IOperator<TEntity> IsFalse();

        IOperator<TEntity> IsNull();
        IOperator<TEntity> IsNotNull();

        IOperator<TEntity> IsEqualTo(Func<TEntity, object> value);
        IOperator<TEntity> IsEqualTo(object value);

        IOperator<TEntity> IsNotEqualTo(Func<TEntity, object> value);
        IOperator<TEntity> IsNotEqualTo(object value);

        IOperator<TEntity> GreaterThan(Func<TEntity, IComparable> value);
        IOperator<TEntity> GreaterThan(IComparable value);

        IOperator<TEntity> LessThan(Func<TEntity, IComparable> value);
        IOperator<TEntity> LessThan(IComparable value);

        IOperator<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IOperator<TEntity> GreaterThanOrEqualTo(IComparable value);

        IOperator<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IOperator<TEntity> LessThanOrEqualTo(IComparable value);

        //IIfConditionSatisfied<TEntity> IsInstanceOf(Func<TEntity, IComparable> value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf(Type value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf<T>();
    }
}
