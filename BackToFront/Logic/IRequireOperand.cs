using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequireOperand<TEntity, TViolation> : IOperandBase<TEntity, TViolation>
    {
        IRule<TEntity, TViolation> OrModelViolationIs(TViolation violation);
    }
}
