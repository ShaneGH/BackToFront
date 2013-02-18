using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class SubRule<TEntity, TViolation>: ISubRule<TEntity, TViolation>
    {
        public IOperators<TEntity, TViolation> If(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }

        public IRequirement<TEntity, TViolation> RequireThat(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }

        public IViolation Validate(object subject)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IViolation> ValidateAll(object subject)
        {
            throw new NotImplementedException();
        }

        public IViolation Validate(TEntity subject)
        {
            throw new NotImplementedException();
        }

        public void ValidateAll(TEntity subject, IList<IViolation> violationList)
        {
            throw new NotImplementedException();
        }

        public Base.IPathElement NextOption
        {
            get { throw new NotImplementedException(); }
        }
    }
}
