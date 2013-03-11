using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Extensions.Reflection
{
    public static class Reflection
    {
        /// <summary>
        /// Mimicks "this is parentClass"
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parentClass"></param>
        /// <returns></returns>
        public static bool Is(this Type target, Type parentClass)
        {
            return target == parentClass || target.IsSubclassOf(parentClass);
        }

        /// <summary>
        /// Mimicks "this is parentClass"
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parentClass"></param>
        /// <returns></returns>
        public static bool Is<T>(this Type target)
        {
            return Is(target, typeof(T));
        }
    }
}
