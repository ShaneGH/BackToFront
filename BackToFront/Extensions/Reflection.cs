using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Extensions
{
    public static class Reflection
    {
        public static bool Is(this Type target, Type parentClass)
        {
            return target == parentClass || target.IsSubclassOf(parentClass);
        }
    }
}
