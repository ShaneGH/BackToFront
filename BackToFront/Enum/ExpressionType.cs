using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Enum
{
    public enum ExpressionWrapperType
    {
        Binary,
        Constant,
        DependencyInjection,
        //ElementAt,
        //Invocation,
        Member,
        MethodCall,
        Parameter,
        Unary
    }
}
