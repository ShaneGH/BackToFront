using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Validation
{
    public interface IRuleValidation<TEntity> : IValidate<TEntity>, INonGenericRule
    {
    }
}
