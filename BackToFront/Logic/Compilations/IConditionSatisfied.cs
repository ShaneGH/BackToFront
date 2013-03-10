using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic.Compilations
{
    public interface ISmartConditionSatisfied<TEntity> : IModelViolation1<TEntity>, IRequires<TEntity>, IBeginSubRule<TEntity>
    {
    }

    /// <summary>
    /// Signifies that a condition has been satisfied and includes the options to continue
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IConditionSatisfied<TEntity> : ISmartConditionSatisfied<TEntity>, IAddCondition<TEntity>
    {
    }
}
