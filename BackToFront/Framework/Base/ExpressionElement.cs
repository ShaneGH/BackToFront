using BackToFront.Expressions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class which holds reference to a a piece of code
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

            Descriptor = ExpressionWrapperBase.ToWrapper(descriptor, out Parameters);
        }

        public Func<TEntity, object[], TMember> Compile(IEnumerable<Utils.Mock> mocks)
        {
            // add extra parameter for mock values
            var parameters = Parameters.Concat(new[] { Expression.Parameter(typeof(object[])) });
            return Expression.Lambda<Func<TEntity, object[], TMember>>(Descriptor.Compile(mocks), parameters).Compile();
        }
    }
}
