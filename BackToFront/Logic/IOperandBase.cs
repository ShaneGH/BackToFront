using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    //Violation, Require, Then, And
    public interface IOperandBase<TEntity, TViolation>
    {
        IOperators<TEntity, TViolation> And(Func<TEntity, object> value);

        IOperators<TEntity, TViolation> Or(Func<TEntity, object> value);

        IRule<TEntity, TViolation> Then(Action<ISubRule<TEntity, TViolation>> action);
    }
}
