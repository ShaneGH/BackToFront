using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Utils;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class which holds reference to a property. Also has a lockable action
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class ExpressionElement<TEntity> : PathElement<TEntity>
    {
        protected readonly Func<TEntity, object> Descriptor;

        protected ExpressionElement(Expression<Func<TEntity, object>> descriptor, Rule<TEntity> rule)
            : base(rule)
        {
            if (descriptor == null)
                throw new ArgumentNullException("##4");

            Descriptor = descriptor.Compile();
        }
    }
}
