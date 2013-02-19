using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequireOperators<TEntity>
    {
        IModelViolation2<TEntity> IsTrue();
        IModelViolation2<TEntity> IsFalse();

        IModelViolation2<TEntity> IsNull();
        IModelViolation2<TEntity> IsNotNull();

        IModelViolation2<TEntity> IsEqualTo(Func<TEntity, object> value);
        IModelViolation2<TEntity> IsEqualTo(object value);

        IModelViolation2<TEntity> IsNotEqualTo(Func<TEntity, object> value);
        IModelViolation2<TEntity> IsNotEqualTo(object value);

        IModelViolation2<TEntity> GreaterThan(Func<TEntity, IComparable> value);
        IModelViolation2<TEntity> GreaterThan(IComparable value);

        IModelViolation2<TEntity> LessThan(Func<TEntity, IComparable> value);
        IModelViolation2<TEntity> LessThan(IComparable value);

        IModelViolation2<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IModelViolation2<TEntity> GreaterThanOrEqualTo(IComparable value);

        IModelViolation2<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IModelViolation2<TEntity> LessThanOrEqualTo(IComparable value);

        //IIfConditionSatisfied<TEntity> IsInstanceOf(Func<TEntity, IComparable> value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf(Type value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf<T>();
    }
}
