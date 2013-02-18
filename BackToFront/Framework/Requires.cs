using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class Requires<TEntity, TViolation>: IRequires<TEntity, TViolation>
    {
        public IRequirement<TEntity, TViolation> RequireThat(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }
    }
}
