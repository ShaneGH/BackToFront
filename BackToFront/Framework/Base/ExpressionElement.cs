using BackToFront.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;
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
        public readonly ExpressionWrapperBase Descriptor;
        protected readonly ReadOnlyCollection<ParameterExpression> Parameters;
        public readonly ParameterExpression EntityParameter;

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

        public override IEnumerable<MemberChainItem> ValidationSubjects
        {
            get
            {
                return PropertyRequirement ?
                    Descriptor.GetMembersForParameter(EntityParameter) :
                    Enumerable.Empty<MemberChainItem>();
            }
        }

        public override IEnumerable<MemberChainItem> RequiredForValidation
        {
            get
            {
                return PropertyRequirement ?
                    Enumerable.Empty<MemberChainItem>() :
                    Descriptor.GetMembersForParameter(EntityParameter);
            }
        }
    }
}
