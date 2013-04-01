using BackToFront.Expressions;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        protected readonly ParameterExpression EntityParameter;

        protected ExpressionElement(Expression<Func<TEntity, TMember>> descriptor, Rule<TEntity> rule)
            : base(rule)
        {
            if (descriptor == null)
                throw new ArgumentNullException("##4");

            Descriptor = ExpressionWrapperBase.ToWrapper(descriptor, out Parameters);
            if (Parameters.Count < 1 || Parameters[0].Type != typeof(TEntity))
                throw new InvalidOperationException("##");

            EntityParameter = Parameters.First();
        }

        public CompiledMockedExpression<TEntity, TMember> Compile()
        {
            return Compile(new Mocks());
        }

        public CompiledMockedExpression<TEntity, TMember> Compile(Mocks mocks)
        {
            // add extra parameter for mock values
            var parameters = Parameters.Concat(new[] { mocks.Parameter });
            var compiled = Expression.Lambda<Func<TEntity, object[], TMember>>(Descriptor.Compile(mocks), parameters).Compile();
            return new CompiledMockedExpression<TEntity, TMember>(compiled, mocks);
        }

        public override IEnumerable<MemberChainItem> AffectedMembers
        {
            get 
            {
                return Descriptor.GetMembersForParameter(EntityParameter);
            }
        }
    }
}
