using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions.Expressions;

namespace BackToFront.Framework.Base
{
    /// <summary>
    /// A class which holds reference to a property. Also has a lockable action
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class PropertyElement<TEntity>
    {
        /// <summary>
        /// Use instead of null for un needed constructor argument
        /// </summary>
        internal static readonly Expression<Func<TEntity, object>> IgnorePointer = a => a;
        private bool _locked = false;
        protected readonly Func<TEntity, object> Descriptor;
        protected readonly IEnumerable<MemberInfo> Members;
        
        protected PropertyElement(Expression<Func<TEntity, object>> descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("##");

            if (descriptor != IgnorePointer)
            {
                var affects = descriptor.AsProperty();
                // TODO: this
                Members = affects/*.SwapMethodsForTheirAffectedProperties()*/;
            }

            Descriptor = descriptor.Compile();
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
                throw new InvalidOperationException("##");
            else
                _locked = true;
        }
    }
}
