using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Attributes;
using BackToFront.Extensions.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Utils;

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
        protected readonly PropertyChain<TEntity> Chain;
        public bool HasValidChain
        {
            get
            {
                return Chain != null;
            }
        }

        protected PropertyElement(Expression<Func<TEntity, object>> descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("##4");

            if (descriptor != IgnorePointer)
            {
                Chain = CompileChain(descriptor, out Members);
            }

            Descriptor = descriptor.Compile();
        }

        /// <summary>
        /// Returns a valid chain of Fields and properties, or returns null (if this is impossible)
        /// </summary>
        /// <param name="memberChain"></param>
        /// <returns></returns>
        private static PropertyChain<TEntity> CompileChain(Expression<Func<TEntity, object>> descriptor, out IEnumerable<MemberInfo> affectedMembers)
        {
            var properties = descriptor.ReferencedProperties().ToArray();
            affectedMembers = DistillMemberInfo(properties);
            try
            {
                return new PropertyChain<TEntity>(properties);
            }
            catch (EX)
            {
                return null;
            }
        }

        /// <summary>
        /// Converts an array of members into an array of PropertyInfo and FieldInfo, taking into account the AffectsMembersAttribute
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        private static IEnumerable<MemberInfo> DistillMemberInfo(MemberInfo[] members)
        {
            foreach (var member in members)
            {
                if (member is PropertyInfo || member is FieldInfo)
                    yield return member;

                var attr = (AffectsMembersAttribute)member.GetCustomAttribute(typeof(AffectsMembersAttribute));
                if (attr != null)
                {
                    foreach (var affectedMembers in attr.GetMembers(typeof(TEntity)))
                        yield return affectedMembers;
                }
            }
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
