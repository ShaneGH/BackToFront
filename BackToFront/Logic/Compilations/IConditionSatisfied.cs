using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic.Compilations
{
    /// <summary>
    /// Signifies that a condition has been satisfied and includes the options to continue
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IConditionSatisfied<TEntity> : IModelViolation1<TEntity>, IAddCondition<TEntity>, IRequires<TEntity>, IBeginSubRule<TEntity>
    {
    }
}
