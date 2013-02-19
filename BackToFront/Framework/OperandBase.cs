using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class OperandBase<TEntity> : IOperandBase<TEntity>
    {
        public IOperators<TEntity> And(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }

        public IOperators<TEntity> Or(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }

        public IRule<TEntity> Then(Action<ISubRule<TEntity>> action)
        {
            throw new NotImplementedException();
        }
    }
}
