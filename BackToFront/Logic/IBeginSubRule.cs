using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IBeginSubRule<TEntity>
    {
        IAdditionalRuleCondition<TEntity> Then(Action<IRule<TEntity>> action);
    }
}
