using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Enum
{
    public enum MemberIndexType
    {
        /// <summary>
        /// A specific element in the array is being referred to
        /// </summary>
        Explicit,

        /// <summary>
        /// No 1 element in the array is referred to, just that the next element will be of Type T in IEnumerable&lt;T>
        /// </summary>
        General
    }
}
