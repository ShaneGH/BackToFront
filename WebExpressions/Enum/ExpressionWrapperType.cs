using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExpressions.Enum
{
    public enum ExpressionWrapperType
    {
        Binary = 1,
        Constant = 2,
        MemberX = 3,
        StaticMember = 4,
        MethodCall = 5,
        Parameter = 6,
        Unary = 7,
        Default = 8,
        Block = 9,
        Conditional = 10,
        Invocation = 11,
        New = 12
    }
}
