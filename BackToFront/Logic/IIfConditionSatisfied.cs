using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IOperator<TEntity, TViolation> : IOperandBase<TEntity, TViolation>, IRequires<TEntity, TViolation>
    {
        IRule<TEntity, TViolation> ModelViolationIs(TViolation violation);
    }
}
