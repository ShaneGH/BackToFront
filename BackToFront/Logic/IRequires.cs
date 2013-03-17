using BackToFront.Logic.Compilations;
using System;
using System.Linq.Expressions;

namespace BackToFront.Logic
{
    public interface IRequires<TEntity>
    {
        IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> condition);
    }
}
