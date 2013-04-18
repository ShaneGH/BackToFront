using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utilities
{
    public class MemberChainItem
    {
        private static readonly Type _IEnumerable = typeof(IEnumerable<>);
        public readonly Type IndexedType;
        public readonly MemberIndex Index;
        public readonly MemberInfo Member;
        private MemberChainItem _NextItem;
        public MemberChainItem NextItem
        {
            get
            {
                return _NextItem;
            }
            set
            {
                if (value != _NextItem)
                {
                    if (value != null)
                    {
                        var type = Index != null ? IndexedType : Member.MemberType();
                        if (value.Member.DeclaringType != type)
                            throw new InvalidOperationException("##");
                    }

                    _NextItem = value;
                }
            }
        }

        public MemberInfo UltimateMember
        {
            get
            {
                // use recursive function to force stack overflow exception in case of circular reference
                Func<MemberChainItem, MemberInfo> result = null;
                result = a => a.NextItem == null ? a.Member : result(a.NextItem);
                
                return result(this);
            }
        }

        public MemberChainItem(MemberInfo member)
            : this(member, null) { }

        public MemberChainItem(MemberInfo member, MemberIndex index)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            if (index != null)
            {
                if (member is Type)
                    throw new InvalidOperationException("A type cannot be indexed"); //"##"
                if (!member.MemberType().Is(_IEnumerable))
                    throw new InvalidOperationException("A non enumerable member cannot be indexed"); //"##"

                IndexedType = member.MemberType().GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == _IEnumerable)
                    .GetGenericArguments()[0];
            }

            Member = member;
            Index = index;
        }

        public override int GetHashCode()
        {
            return (Member.GetHashCode() + "_" + (NextItem == null ? string.Empty : NextItem.GetHashCode().ToString())).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as MemberChainItem;

            if (item == null || item.Member!= Member || item.Index != Index)
                return false;

            return (NextItem == null && item.NextItem == null) || NextItem.Equals(item.NextItem);
        }

        public static bool operator ==(MemberChainItem item1, MemberChainItem item2)
        {
            return Utils.BindOperatorToEquals(item1, item2);
        }

        public static bool operator !=(MemberChainItem item1, MemberChainItem item2)
        {
            return !(item1 == item2);
        }
    }
}
