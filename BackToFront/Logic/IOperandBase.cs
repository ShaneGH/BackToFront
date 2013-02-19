using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    //Violation, Require, Then, And
    public interface IOperandBase<TEntity>
    {
        IOperators<TEntity> And(Func<TEntity, object> value);

        IOperators<TEntity> Or(Func<TEntity, object> value);

        IRule<TEntity> Then(Action<ISubRule<TEntity>> action);
    }
}
