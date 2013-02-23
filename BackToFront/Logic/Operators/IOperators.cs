using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        IConditionSatisfied<TEntity> IsEqualTo(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> IsEqualTo(object value);

        IConditionSatisfied<TEntity> IsNotEqualTo(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> IsNotEqualTo(object value);

        IConditionSatisfied<TEntity> GreaterThan(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> GreaterThan(IComparable value);

        IConditionSatisfied<TEntity> LessThan(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> LessThan(IComparable value);

        IConditionSatisfied<TEntity> GreaterThanOrEqualTo(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> GreaterThanOrEqualTo(IComparable value);

        IConditionSatisfied<TEntity> LessThanOrEqualTo(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> LessThanOrEqualTo(IComparable value);

        IConditionSatisfied<TEntity> IsInstanceOf(Expression<Func<TEntity, object>> value);
        IConditionSatisfied<TEntity> IsInstanceOf(Type value);
        IConditionSatisfied<TEntity> IsInstanceOf<T>();
    }
}
