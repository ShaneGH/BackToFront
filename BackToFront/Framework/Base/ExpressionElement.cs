using BackToFront.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class which holds reference to a property. Also has a lockable action
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ExpressionElement<TEntity, TMember> : PathElement<TEntity>
    {
        protected readonly ExpressionWrapperBase Descriptor;
        protected readonly ReadOnlyCollection<ParameterExpression> Parameters;

        protected ExpressionElement(Expression<Func<TEntity, TMember>> descriptor, Rule<TEntity> rule)
            : base(rule)
        {
            if (descriptor == null)
                throw new ArgumentNullException("##4");

            Descriptor = ExpressionWrapperBase.ToWrapper(descriptor);
            Parameters = descriptor.Parameters;
        }

        public Func<TEntity, TMember> Compile(IEnumerable<Utils.Mock> mocks)
        {
            return Expression.Lambda<Func<TEntity, TMember>>(Descriptor.Evaluate(mocks), Parameters).Compile();
        }
    }
}
