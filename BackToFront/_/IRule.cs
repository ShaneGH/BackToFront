using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Compilations;

namespace BackToFront
{
    /// <summary>
    /// A business rule
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IRule<TEntity>
    {
        IOperators<TEntity> If(Expression<Func<TEntity, object>> property);
    }

    public interface IElse<TEntity>
    {
        IOperators<TEntity> ElseIf(Expression<Func<TEntity, object>> property);
        IConditionSatisfied<TEntity> Else { get; }
    }
}
