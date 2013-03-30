using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

namespace BackToFront.Framework.Base
{
    internal class DeadEnd<TEntity> : PathElement<TEntity>
    {
        public DeadEnd()
            : base(null) { }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, Utils.Mocks mocks)
        {
            yield break;
        }

        public override IViolation ValidateEntity(TEntity subject, Utils.Mocks mocks)
        {
            return null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            return;
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
        public abstract IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, Utils.Mocks mocks);
        private static readonly DeadEnd<TEntity> _DeadEnd = new DeadEnd<TEntity>();

        public PathElement<TEntity> NextOption(TEntity subject, Utils.Mocks mocks)
        {
            var options = NextPathElements(subject, mocks).Where(a => a != null).ToArray();
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
        }

        public virtual IViolation ValidateEntity(TEntity subject, Utils.Mocks mocks)
        {
            return NextOption(subject, mocks).ValidateEntity(subject, mocks);
        }

        public virtual void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            NextOption(subject, mocks).FullyValidateEntity(subject, violationList, mocks);
        }

        ///// <summary>
        ///// Validate the next element in the chain or return no violation if no more elements
        ///// </summary>
        ///// <param name="subject"></param>
        ///// <returns></returns>
        //protected IViolation ValidateNext(TEntity subject, Utils.Mocks mocks)
        //{
        //    //TODO: make private and handle next logic here (rather than in child)
        //    var no = NextOption(subject, mocks);
        //    if (no == null)
        //        return null;

        //    return no.ValidateEntity(subject, mocks);
        //}

        ///// <summary>
        ///// Validate the next element in the chain or return no violation if no more elements
        ///// </summary>
        ///// <param name="subject"></param>
        ///// <param name="violations"></param>
        //protected void ValidateAllNext(TEntity subject, IList<IViolation> violations, Utils.Mocks mocks)
        //{
        //    //TODO: make private and handle next logic here (rather than in child)
        //    var no = NextOption(subject, mocks);
        //    if(no != null)
        //        no.FullyValidateEntity(subject, violations, mocks);
        //}

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
