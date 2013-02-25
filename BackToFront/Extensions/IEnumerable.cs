using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Extensions.IEnumerable
{
    public static class IEnumerable
    {
        /// <summary>
        /// Foreach statement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            var enumerated = items.ToArray();
            for (int i = 0, ii = enumerated.Length; i < ii; i++)
                action(enumerated[i]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            var enumerated = items.ToArray();
            for (int i = 0, ii = enumerated.Length; i < ii; i++)
                action(enumerated[i], i);
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            var enumerated = items.ToArray();
            for (int i = 0, ii = enumerated.Length; i < ii; i++)
                list.Add(enumerated[i]);
        }

        public static IEnumerable<T> Aggregate<T>(this IEnumerable<IEnumerable<T>> elements)
        {
            if (elements.Count() > 0)
            {
                return elements.Aggregate((a, b) => a.Union(b));
            }

            return Enumerable.Empty<T>();
        }
    }
}
