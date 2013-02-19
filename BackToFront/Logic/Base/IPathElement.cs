using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic.Base
{
    public interface IPathElement<TEntity> : IValidate<TEntity>
    {
        IPathElement<TEntity> NextOption { get; }
    }
}
