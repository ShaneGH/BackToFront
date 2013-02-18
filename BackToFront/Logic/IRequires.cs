using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequires<TEntity, TViolation>
    {
        IRequirement<TEntity, TViolation> RequireThat(Func<TEntity, object> property);
    }
}
