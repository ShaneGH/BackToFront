using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Utilities;

namespace BackToFront.Logic.Conditions
{
    public interface IAdditionalOperators<TEntity>
    {
        IAdditionalCondition<TEntity> IsTrue();
        IAdditionalCondition<TEntity> IsFalse();

        IAdditionalCondition<TEntity> IsNull();
        IAdditionalCondition<TEntity> IsNotNull();

        IAdditionalCondition<TEntity> IsEqualTo(Func<TEntity, object> value);
        IAdditionalCondition<TEntity> IsEqualTo(object value);

        IAdditionalCondition<TEntity> IsNotEqualTo(Func<TEntity, object> value);
        IAdditionalCondition<TEntity> IsNotEqualTo(object value);

        IAdditionalCondition<TEntity> GreaterThan(Func<TEntity, IComparable> value);
        IAdditionalCondition<TEntity> GreaterThan(IComparable value);

        IAdditionalCondition<TEntity> LessThan(Func<TEntity, IComparable> value);
        IAdditionalCondition<TEntity> LessThan(IComparable value);

        IAdditionalCondition<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IAdditionalCondition<TEntity> GreaterThanOrEqualTo(IComparable value);

        IAdditionalCondition<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IAdditionalCondition<TEntity> LessThanOrEqualTo(IComparable value);

        IAdditionalCondition<TEntity> IsInstanceOf(Func<TEntity, Type> value);
        IAdditionalCondition<TEntity> IsInstanceOf(Type value);
        IAdditionalCondition<TEntity> IsInstanceOf<T>();
    }
}
