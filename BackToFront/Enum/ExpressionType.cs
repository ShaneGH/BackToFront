using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Enum
{
    public enum ExpressionWrapperType
    {
        Binary = 1,
        Constant = 2,
        DependencyInjection = 3,
        //ElementAt,
        //Invocation,
        Member = 4,
        MethodCall = 5,
        Parameter = 6,
        Unary = 7
    }
}
