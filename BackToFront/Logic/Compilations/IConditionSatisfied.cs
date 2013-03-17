using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic.Compilations
{
    public interface IConditionSatisfied<TEntity> : IRequirementFailed2<TEntity>, IRequires<TEntity>, IBeginSubRule<TEntity>
    {
    }
}
