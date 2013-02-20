using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic.Compilations
{
    /// <summary>
    /// Signifies that a reqirement has failed and includes the options to continue
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRequirementFailed<TEntity> : IModelViolation2<TEntity>, IAddRequireCondition<TEntity>//, IRequires<TEntity>
    {
    }
}
