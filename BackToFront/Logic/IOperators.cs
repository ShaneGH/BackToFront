using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Compilations;

namespace BackToFront.Logic
{
    public interface IOperators<TEntity>
    {
        IConditionSatisfied<TEntity> IsTrue();
        IConditionSatisfied<TEntity> IsFalse();

        IConditionSatisfied<TEntity> IsNull();
        IConditionSatisfied<TEntity> IsNotNull();

        IConditionSatisfied<TEntity> IsEqualTo(Func<TEntity, object> value);
        IConditionSatisfied<TEntity> IsEqualTo(object value);

        IConditionSatisfied<TEntity> IsNotEqualTo(Func<TEntity, object> value);
        IConditionSatisfied<TEntity> IsNotEqualTo(object value);

        IConditionSatisfied<TEntity> GreaterThan(Func<TEntity, IComparable> value);
        IConditionSatisfied<TEntity> GreaterThan(IComparable value);

        IConditionSatisfied<TEntity> LessThan(Func<TEntity, IComparable> value);
        IConditionSatisfied<TEntity> LessThan(IComparable value);

        IConditionSatisfied<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IConditionSatisfied<TEntity> GreaterThanOrEqualTo(IComparable value);

        IConditionSatisfied<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IConditionSatisfied<TEntity> LessThanOrEqualTo(IComparable value);

        //IIfConditionSatisfied<TEntity> IsInstanceOf(Func<TEntity, IComparable> value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf(Type value);
        //IIfConditionSatisfied<TEntity> IsInstanceOf<T>();
    }
}
