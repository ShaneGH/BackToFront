using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        IAdditionalCondition<TEntity> IsEqualTo(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> IsEqualTo(object value);

        IAdditionalCondition<TEntity> IsNotEqualTo(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> IsNotEqualTo(object value);

        IAdditionalCondition<TEntity> GreaterThan(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> GreaterThan(IComparable value);

        IAdditionalCondition<TEntity> LessThan(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> LessThan(IComparable value);

        IAdditionalCondition<TEntity> GreaterThanOrEqualTo(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> GreaterThanOrEqualTo(IComparable value);

        IAdditionalCondition<TEntity> LessThanOrEqualTo(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> LessThanOrEqualTo(IComparable value);

        IAdditionalCondition<TEntity> IsInstanceOf(Expression<Func<TEntity, object>> value);
        IAdditionalCondition<TEntity> IsInstanceOf(Type value);
        IAdditionalCondition<TEntity> IsInstanceOf<T>();
    }
}
