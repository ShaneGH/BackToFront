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
        Member = 3,
        StaticMember = 4,
        MethodCall = 5,
        StaticMethodCall = 6,
        Parameter = 7,
        Unary = 8,
        Default = 9,
        Block = 10,
        Conditional = 11,
        Invocation = 12,
        New = 13
    }
}
