using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;
using BackToFront.Logic.Base;

namespace BackToFront.Framework.Base
{
    internal abstract class PathElement<TEntity, TViolation> : RuleChildElement<TEntity, TViolation>, IPathElement
        where TViolation : IViolation
    {
        protected abstract IEnumerable<IValidatablePathElement<TEntity>> NextPathElement { get; }
        public IPathElement NextOption
        {
            get
            {
                var options = NextPathElement.Where(a => a != null).ToArray();
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

        protected PathElement(Func<TEntity, object> descriptor, Rule<TEntity, TViolation> rule)
            : base(descriptor, rule)
        {
        }

        private IPathElement GetNextOption(Type[] skipInstancesOfType = null)
        {
            IPathElement option = NextOption;
            if (skipInstancesOfType != null)
            {
                while (option != null && skipInstancesOfType.Any(a => option.GetType().Is(a)))
                {
                    option = option.NextOption;
                }
            }

            return option;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="skipInstancesOfType">Skip over NextOption if NextOption is Type</param>
        /// <returns></returns>
        protected IViolation ValidateNext(TEntity subject, Type[] skipInstancesOfType = null)
        {
            object option = GetNextOption(skipInstancesOfType);
            return option is IValidatablePathElement<TEntity> ? (option as IValidatablePathElement<TEntity>).Validate(subject) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violations"></param>
        /// <param name="skipInstancesOfType">Skip over NextOption if NextOption is Type</param>
        protected void ValidateAllNext(TEntity subject, IList<IViolation> violations, Type[] skipInstancesOfType = null)
        {
            object option = GetNextOption(skipInstancesOfType);
            if (option is IValidatablePathElement<TEntity>)
                (option as IValidatablePathElement<TEntity>).ValidateAll(subject, violations);
        }
    }
}
