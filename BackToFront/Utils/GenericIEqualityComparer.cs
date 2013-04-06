using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utils
{
    public class GenericIEqualityComparer<T> : IEqualityComparer<T>
    {
        public readonly Func<T, T, bool> Equals;
        public readonly Func<T, int> GetHashCode;

        public GenericIEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            Equals = equals;
            GetHashCode = getHashCode;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return this.Equals(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return this.GetHashCode(obj);
        }
    }
}
