using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

namespace BackToFront.Framework.Base
{
    internal enum ValidateResult
    {
        TerminateHere,
        PassThrough,
        ThowViolation
    }

    /// <summary>
    /// A class attached to a Rule which points to the next step in the operation
    /// </summary>
    /// <typeparam name="TEntity">The entity type to validate</typeparam>
    internal abstract class PathElement<TEntity>
    {
        private bool _locked = false;
        protected readonly Rule<TEntity> ParentRule;
        protected abstract IEnumerable<ExpressionElement<TEntity>> NextPathElements(TEntity subject);

        public ExpressionElement<TEntity> NextOption(TEntity subject)
        {
                var options = NextPathElements(subject).Where(a => a != null).ToArray();
                if (!options.Any())
                {
                    return null;
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

        public abstract void ValidateEntity(TEntity subject, out IViolation violation);
        public abstract void FullyValidateEntity(TEntity subject, IList<IViolation> violationList);

        /// <summary>
        /// Validate the next element in the chain or return no violation if no more elements
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        protected IViolation ValidateNext(TEntity subject)
        {
            //TODO: make private and handle next logic here (rather than in child)
            IViolation violation = null;
            var no = NextOption(subject);
            if(no != null)
                no.ValidateEntity(subject, out violation);
            return violation;
        }

        /// <summary>
        /// Validate the next element in the chain or return no violation if no more elements
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violations"></param>
        protected void ValidateAllNext(TEntity subject, IList<IViolation> violations)
        {
            //TODO: make private and handle next logic here (rather than in child)
            var no = NextOption(subject);
            if(no != null)
                no.FullyValidateEntity(subject, violations);
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
