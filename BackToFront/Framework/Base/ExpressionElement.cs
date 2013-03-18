using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Utils;
using BackToFront.Utils.Expressions;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class which holds reference to a property. Also has a lockable action
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ExpressionElement<TEntity, TMember> : PathElement<TEntity>
    {
        protected readonly ExpressionWrapperBase Descriptor;

        protected ExpressionElement(Expression<Func<TEntity, TMember>> descriptor, Rule<TEntity> rule)
            : base(rule)
        {
            if (descriptor == null)
                throw new ArgumentNullException("##4");

            Descriptor = ExpressionWrapperBase.ToWrapper(descriptor);
        }

        public Func<TEntity, TMember> Compile(IEnumerable<Utils.Mock> mocks)
        {
            return Expression.Lambda<Func<TEntity, TMember>>(Descriptor.Evaluate(mocks), Descriptor.WrappedExpressionParameters).Compile();
        }
    }
}
