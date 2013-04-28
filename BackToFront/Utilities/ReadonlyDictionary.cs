using System;
using System.Collections.Generic;

namespace BackToFront.Utilities
{
    /// <summary>
    /// Wraps a dictionary providing read only access
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReadonlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly IDictionary<TKey, TValue> _inner;
        public ReadonlyDictionary(Dictionary<TKey, TValue> wrappedDictionary)
        {
            if (wrappedDictionary == null)
                throw new ArgumentNullException();

            _inner = wrappedDictionary;
        }

        public bool ContainsKey(TKey key)
        {
            return _inner.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _inner.Keys; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _inner[key];
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_inner).GetEnumerator();
        }
    }
}
