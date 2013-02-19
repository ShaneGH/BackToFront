using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;
using BackToFront.Logic.Base;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class attached to a Rule which points to the next step in the operation
    /// </summary>
    /// <typeparam name="TEntity">The entity type to validate</typeparam>
    internal abstract class PathElement<TEntity> : PropertyElement<TEntity>, IPathElement<TEntity>
    {        
        protected readonly Rule<TEntity> ParentRule;
        protected abstract IEnumerable<IPathElement<TEntity>> NextPathElements { get; }

        public IPathElement<TEntity> NextOption
        {
            get
            {
                var options = NextPathElements.Where(a => a != null).ToArray();
                if (!options.Any())
                {
                    return null;
                }
                else if (options.Length == 1)
                {
                    return options[0];
                }

                throw new InvalidOperationException();
            }
        }

        public PathElement(Func<TEntity, object> descriptor, Rule<TEntity> rule)
            : base(descriptor)
        {
            ParentRule = rule;
        }

        public abstract IViolation ValidateEntity(TEntity subject);
        public abstract void FullyValidateEntity(TEntity subject, IList<IViolation> violationList);

        /// <summary>
        /// Validate the next element in the chain or return no violation if no more elements
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IViolation ValidateNext(TEntity subject)
        {
            var no = NextOption;
            return no == null ? null : no.ValidateEntity(subject);
        }

        /// <summary>
        /// Validate the next element in the chain or return no violation if no more elements
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violations"></param>
        protected void ValidateAllNext(TEntity subject, IList<IViolation> violations)
        {
            var no = NextOption;
            if(no != null)
                no.FullyValidateEntity(subject, violations);
        }
    }
}
