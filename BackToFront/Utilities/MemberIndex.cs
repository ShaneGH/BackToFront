using BackToFront.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utilities
{
    public class MemberIndex
    {
        public readonly int Index;
        public readonly MemberIndexType Type;

        public MemberIndex()
            : this(0, MemberIndexType.General) { }

        public MemberIndex(int index)
            : this(index, MemberIndexType.Explicit) { }

        private MemberIndex(int index, MemberIndexType type)
        {
            Index = index;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var memberIndex = obj as MemberIndex;
            return memberIndex != null && memberIndex.Index == Index && memberIndex.Type == Type;
        }

        public override int GetHashCode()
        {
            return (Index.GetHashCode() + "_" + Type.GetHashCode()).GetHashCode();
        }

        public static bool operator ==(MemberIndex item1, MemberIndex item2)
        {
            return Utils.BindOperatorToEquals(item1, item2);
        }

        public static bool operator !=(MemberIndex item1, MemberIndex item2)
        {
            return !(item1 == item2);
        }
    }
}
