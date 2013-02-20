using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IModelViolation2<TEntity>
    {
        IRule<TEntity> OrModelViolationIs(IViolation violation);
    }    
}
