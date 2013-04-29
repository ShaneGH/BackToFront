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
    public class AffectedMembers
    {
        public MemberChainItem Member { get; set; }
        public bool Requirement { get; set; }
    }

    /// <summary>
    /// A class attached to a Rule which points to the next step in the operation
    /// </summary>
    /// <typeparam name="TEntity">The entity type to validate</typeparam>
    public abstract class PathElement<TEntity> : IValidate<TEntity>
    {
        private bool _locked = false;
        protected readonly Rule<TEntity> ParentRule;
        private static readonly DeadEnd<TEntity> _DeadEnd = new DeadEnd<TEntity>();

        public abstract IEnumerable<AffectedMembers> AffectedMembers { get; }
        public abstract bool PropertyRequirement { get; }

        public static Expression<Action<TEntity, ValidationContextX>> DoNothing = 
            Expression.Lambda<Action<TEntity, ValidationContextX>>(Expression.Empty(), Expression.Parameter(typeof(TEntity)), Expression.Parameter(typeof(ValidationContextX)));

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

        public abstract PathElementMeta Meta { get; }

        public Expression NewCompile(SwapPropVisitor visitor)
        {
            var nc = _NewCompile(visitor);
            var _break = typeof(ValidationContextX).GetProperty("Break");

            return Expression.IfThen(Expression.Not(Expression.Property(visitor.ContextParameter, _break)), nc);
        }

        protected abstract Expression _NewCompile(SwapPropVisitor visitor);
    }
}
