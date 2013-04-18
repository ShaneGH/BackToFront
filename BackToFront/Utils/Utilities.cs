using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utils
{
    public static class Utilities
    {
        public static bool BindOperatorToEquals<T>(T item1, T item2)
        {
            int? i1Hash = null;
            int? i2Hash = null;

            try
            {
                i1Hash = item1.GetHashCode();
            }
            catch (NullReferenceException)
            {
            }

            try
            {
                i2Hash = item2.GetHashCode();
            }
            catch (NullReferenceException)
            {
            }

            if (!i1Hash.HasValue && !i2Hash.HasValue)
                return true;

            if (!i1Hash.HasValue || !i2Hash.HasValue)
                return false;

            if (i1Hash.Value != i2Hash.Value)
                return false;

            return item1.Equals(item2);
        }
    }
}
