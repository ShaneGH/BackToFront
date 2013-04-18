using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utilities
{
    public class GenericIEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _Equals;
        private readonly Func<T, int> _GetHashCode;

        public GenericIEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            _Equals = equals;
            _GetHashCode = getHashCode;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return this._Equals(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return this._GetHashCode(obj);
        }
    }
}
