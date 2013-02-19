using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class Requires<TEntity>: IRequires<TEntity>
    {
        public IRequireOperators<TEntity> RequireThat(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }
    }
}
