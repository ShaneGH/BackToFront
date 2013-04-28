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
        public abstract IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context);
        private static readonly DeadEnd<TEntity> _DeadEnd = new DeadEnd<TEntity>();

        public abstract IEnumerable<AffectedMembers> AffectedMembers { get; }
        public abstract bool PropertyRequirement { get; }

        public static Action<TEntity, ValidationContextX> DoNothing = (a, b) => { };

        public abstract IEnumerable<PathElement<TEntity>> AllPossiblePaths { get; }

        public PathElement<TEntity> NextOption(TEntity subject, ValidationContext context)
        {
            var options = NextPathElements(subject, context).Where(a => a != null).ToArray();
            if (options.Length == 0)
            {
                return _DeadEnd;
            }
            else if (options.Length == 1)
            {
                return options[0];
            }

            throw new InvalidOperationException("##3");
        }

        public PathElement(Rule<TEntity> rule)
        {
            ParentRule = rule;
            if (ParentRule != null)
                ParentRule.Register(this);
        }

        public virtual IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return NextOption(subject, context).ValidateEntity(subject, context);
        }

        public virtual void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            NextOption(subject, context).FullyValidateEntity(subject, violationList, context);
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

        public virtual Action<TEntity, ValidationContextX> NewCompile(SwapPropVisitor visitor)
        {
            var nc = _NewCompile(visitor);

            return (entity, context) =>
            {
                if (!context.Break)
                    nc(entity, context);
            };
        }

        protected abstract Action<TEntity, ValidationContextX> _NewCompile(SwapPropVisitor visitor);
    }
}
