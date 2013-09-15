using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using BackToFront.Extensions;
using BackToFront.Meta;
using BackToFront.Utilities;
using BackToFront.Validation;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class attached to a Rule which points to the next step in the operation
    /// </summary>
    /// <typeparam name="TEntity">The entity type to validate</typeparam>
    public abstract class PathElement<TEntity> : IValidate<TEntity>
    {
        private bool _locked = false;
        protected readonly Rule<TEntity> ParentRule;
        private static readonly DeadEnd<TEntity> _DeadEnd = new DeadEnd<TEntity>();

        public virtual IEnumerable<MemberChainItem> ValidationSubjects { get { return Enumerable.Empty<MemberChainItem>(); } }
        public virtual IEnumerable<MemberChainItem> RequiredForValidation { get { return Enumerable.Empty<MemberChainItem>(); } }

        public abstract IEnumerable<PathElement<TEntity>> AllPossiblePaths { get; }

        public PathElement(Rule<TEntity> rule)
        {
            ParentRule = rule;
            if (ParentRule != null)
                ParentRule.Register(this);
        }

        protected TOutput Do<TOutput>(Func<TOutput> action)
        {
            _CheckLock();
            return action();
        }

        protected void Do(Action action)
        {
            _CheckLock();
            action();
        }

        private void _CheckLock()
        {
            if (_locked)
                throw new InvalidOperationException("##5");
            else
                _locked = true;
        }

        public Expression Compile(ExpressionMocker visitor)
        {
            var nc = _Compile(visitor);
            var _break = typeof(ValidationContext).GetProperty("Break");

            return Expression.IfThen(Expression.Not(Expression.Property(visitor.ContextParameter, _break)), nc ?? Expression.Empty());
        }

        protected abstract Expression _Compile(ExpressionMocker visitor); 
    }
}
