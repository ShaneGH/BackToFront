using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;
using BackToFront.Utils;

namespace BackToFront.Framework.Base
{
    internal class DeadEnd<TEntity> : PathElement<TEntity>
    {
        public DeadEnd()
            : base(null) { }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            yield break;
        }

        public override IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            return;
        }

        public override IEnumerable<MemberChainItem> AffectedMembers
        {
            get
            {
                yield break;
            }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }
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

        public abstract IEnumerable<MemberChainItem> AffectedMembers { get; }
        public abstract bool PropertyRequirement { get; }

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
    }
}
