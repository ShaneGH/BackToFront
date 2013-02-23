using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Compilations;

namespace BackToFront.Logic
{
    public interface IRequireOperators<TEntity>
    {
        IRequirementFailed<TEntity> IsTrue();
        IRequirementFailed<TEntity> IsFalse();

        IRequirementFailed<TEntity> IsNull();
        IRequirementFailed<TEntity> IsNotNull();

        IRequirementFailed<TEntity> IsEqualTo(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> IsEqualTo(object value);

        IRequirementFailed<TEntity> IsNotEqualTo(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> IsNotEqualTo(object value);

        IRequirementFailed<TEntity> GreaterThan(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> GreaterThan(IComparable value);

        IRequirementFailed<TEntity> LessThan(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> LessThan(IComparable value);

        IRequirementFailed<TEntity> GreaterThanOrEqualTo(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> GreaterThanOrEqualTo(IComparable value);

        IRequirementFailed<TEntity> LessThanOrEqualTo(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> LessThanOrEqualTo(IComparable value);

        IRequirementFailed<TEntity> IsInstanceOf(Expression<Func<TEntity, object>> value);
        IRequirementFailed<TEntity> IsInstanceOf(Type value);
        IRequirementFailed<TEntity> IsInstanceOf<T>();
    }
}
