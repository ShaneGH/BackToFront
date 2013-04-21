using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Expressions
{
    public interface ILinearExpression
    {
        ExpressionWrapperBase Root { get; }

        ExpressionWrapperBase WithAlternateRoot<TEntity, TChild>(Expression root, Expression<Func<TEntity, TChild>> child);
    }

    public interface IPropertyChainGetter : ILinearExpression
    {
        // TODO: generics here
        object Get(object root);
    }

    public interface IPropertyChain : IPropertyChainGetter
    {
        bool CanSet { get; }

        // TODO: generics here
        void Set(object root, object value);
    }
}
