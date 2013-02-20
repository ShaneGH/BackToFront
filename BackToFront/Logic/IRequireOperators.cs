using System;
using System.Collections.Generic;
using System.Linq;
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

        IRequirementFailed<TEntity> IsEqualTo(Func<TEntity, object> value);
        IRequirementFailed<TEntity> IsEqualTo(object value);

        IRequirementFailed<TEntity> IsNotEqualTo(Func<TEntity, object> value);
        IRequirementFailed<TEntity> IsNotEqualTo(object value);

        IRequirementFailed<TEntity> GreaterThan(Func<TEntity, IComparable> value);
        IRequirementFailed<TEntity> GreaterThan(IComparable value);

        IRequirementFailed<TEntity> LessThan(Func<TEntity, IComparable> value);
        IRequirementFailed<TEntity> LessThan(IComparable value);

        IRequirementFailed<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value);
        IRequirementFailed<TEntity> GreaterThanOrEqualTo(IComparable value);

        IRequirementFailed<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value);
        IRequirementFailed<TEntity> LessThanOrEqualTo(IComparable value);

        //IRequirementFailed<TEntity> IsInstanceOf(Func<TEntity, IComparable> value);
        //IRequirementFailed<TEntity> IsInstanceOf(Type value);
        //IRequirementFailed<TEntity> IsInstanceOf<T>();
    }
}
