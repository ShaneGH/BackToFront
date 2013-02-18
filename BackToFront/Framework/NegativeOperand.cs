using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class NegativeOperand<TEntity, TViolation>:  IRequireOperand<TEntity, TViolation>
    {
        public IRule<TEntity, TViolation> OrModelViolationIs(TViolation violation)
        {
            throw new NotImplementedException();
        }

        public IOperators<TEntity, TViolation> And(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }

        public IOperators<TEntity, TViolation> Or(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }

        public IRule<TEntity, TViolation> Then(Action<ISubRule<TEntity, TViolation>> action)
        {
            throw new NotImplementedException();
        }
    }
}
