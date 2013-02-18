using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class Requirement<TEntity, TViolation> : IRequirement<TEntity, TViolation>
    {
        public IRequireOperand<TEntity, TViolation> IsEqualTo(object value)
        {
            throw new NotImplementedException();
        }

        public IRequireOperand<TEntity, TViolation> IsEqualTo(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }
    }
}
