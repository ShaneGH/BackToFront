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
        MethodCall = 4,
        Parameter = 5,
        Unary = 6,
        Default = 7,
        Block = 8,
        Conditional = 9,
        Invocation = 10,
        New = 11
    }
}
