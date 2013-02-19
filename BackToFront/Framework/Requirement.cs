using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class Requirement<TEntity> : IRequirement<TEntity>
    {
        public IRequireOperand<TEntity> IsEqualTo(object value)
        {
            throw new NotImplementedException();
        }

        public IRequireOperand<TEntity> IsEqualTo(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }
    }
}
