using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IBeginSubRule<TEntity>
    {
        IRule<TEntity> Then(Action<ISubRule<TEntity>> action);
    }
}
