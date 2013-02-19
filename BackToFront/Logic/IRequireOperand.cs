using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequireOperand<TEntity> : IOperandBase<TEntity>
    {
        IRule<TEntity> OrModelViolationIs(IViolation violation);
    }
}
