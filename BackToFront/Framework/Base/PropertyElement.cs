using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

namespace BackToFront.Framework.Base
{
    internal abstract class PropertyElement<TEntity>
    {
        /// <summary>
        /// Use instead of null for un needed constructor argument
        /// </summary>
        internal static readonly Func<TEntity, object> IgnorePointer = a => a;
        private bool _locked = false;
        protected readonly Func<TEntity, object> Descriptor;
        protected readonly PropertyInfo Property;
        
        protected PropertyElement(Func<TEntity, object> descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException();

            Descriptor = descriptor;

            // TODO: this
            Property = Descriptor != IgnorePointer ?
                Descriptor.AsProperty() :
                null;
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
