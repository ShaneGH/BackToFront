using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;
using BackToFront.Logic.Base;

namespace BackToFront.Framework.Base
{
    internal abstract class PathElement<TEntity> : RuleChildElement<TEntity>, IPathElement
    {
        protected abstract IEnumerable<IPathElement> NextPathElement { get; }
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

        protected PathElement(Func<TEntity, object> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="skipInstancesOfType">Skip over NextOption if NextOption is Type</param>
        /// <returns></returns>
        protected IViolation ValidateNext(TEntity subject)
        {
            object option = NextOption;
            return option is IValidate<TEntity> ? (option as IValidate<TEntity>).Validate(subject) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="violations"></param>
        /// <param name="skipInstancesOfType">Skip over NextOption if NextOption is Type</param>
        protected void ValidateAllNext(TEntity subject, IList<IViolation> violations)
        {
            object option = NextOption;
            if (option is IValidate<TEntity>)
                (option as IValidate<TEntity>).ValidateAll(subject, violations);
        }
    }
}
