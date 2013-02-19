using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Base;

namespace BackToFront
{
    /// <summary>
    /// A business rule
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IRule<TEntity>
    {
        IOperators<TEntity> If(Func<TEntity, object> property);
    }
}
