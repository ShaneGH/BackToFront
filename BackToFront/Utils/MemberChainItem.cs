using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils
{
    public class MemberChainItem
    {
        private static readonly Type _IEnumerable = typeof(IEnumerable<>);
        public readonly Type IndexedType;
        public readonly int? Index;
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
                        var type = Index.HasValue ? IndexedType : Member.MemberType();
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
        
        public MemberChainItem(MemberInfo member, int? index)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            if (index.HasValue)
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
            int? i1Hash = null;
            int? i2Hash = null;

            try
            {
                i1Hash = item1.GetHashCode();
            }
            catch (NullReferenceException)
            {
            }

            try
            {
                i2Hash = item2.GetHashCode();
            }
            catch (NullReferenceException)
            {
            }

            if (!i1Hash.HasValue && !i2Hash.HasValue)
                return true;

            if (!i1Hash.HasValue || !i2Hash.HasValue)
                return false;

            if (i1Hash.Value != i2Hash.Value)
                return false;

            return item1.Equals(item2);
        }

        public static bool operator !=(MemberChainItem item1, MemberChainItem item2)
        {
            return !(item1 == item2);
        }
    }
}
