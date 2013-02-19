using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IOperators<TEntity>
    {
        IModelViolation1<TEntity> IsTrue();
        IModelViolation1<TEntity> IsFalse();

        IModelViolation1<TEntity> IsNull();
        IModelViolation1<TEntity> IsNotNull();

        IModelViolation1<TEntity> IsEqualTo(Func<TEntity, object> value);
        IModelViolation1<TEntity> IsEqualTo(object value);

        IModelViolation1<TEntity> IsNotEqualTo(Func<TEntity, object> value);
        IModelViolation1<TEntity> IsNotEqualTo(object value);

        IModelViolation1<TEntity> GreaterThan(Func<TEntity, IComparable> value);
        IModelViolation1<TEntity> GreaterThan(IComparable value);

        IModelViolation1<TEntity> LessThan(Func<TEntity, IComparable> value);
        IModelViolation1<TEntity> LessThan(IComparable value);

        IModelViolation1<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IModelViolation1<TEntity> GreaterThanOrEqualTo(IComparable value);

        IModelViolation1<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IModelViolation1<TEntity> LessThanOrEqualTo(IComparable value);

        //IIfConditionSatisfied<TEntity> IsInstanceOf(Func<TEntity, IComparable> value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf(Type value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf<T>();
    }
}
