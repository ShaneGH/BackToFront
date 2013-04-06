using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utils
{
    public class ReflectionWrapper
    {
        private static readonly Dictionary<Tuple<Type, string>, PropertyInfo> CachedProperties = new Dictionary<Tuple<Type, string>, PropertyInfo>();
        private static readonly Dictionary<Tuple<Type, string>, FieldInfo> CachedFields = new Dictionary<Tuple<Type, string>, FieldInfo>();
        private static readonly Dictionary<Tuple<Type, string, Type[]>, MethodInfo> CachedMethods = new Dictionary<Tuple<Type, string, Type[]>, MethodInfo>(new MethodComparer());

        private readonly object Item;
        private readonly Type Type;

        private static bool _UseCache = true;

        /// <summary>
        /// If set to true, pointers for all methods, properties and fields are cached statically. Reduces reflection overhead
        /// </summary>
        public static bool UseCache
        {
            get
            {
                return _UseCache;
            }
            set
            {
                _UseCache = value;
                if (!_UseCache)
                {
                    CachedProperties.Clear();
                    CachedFields.Clear();
                    CachedMethods.Clear();
                }
            }
        }

        public ReflectionWrapper(object item)
        {
            if (item == null)
                throw new ArgumentNullException();

            Item = item;
            Type = item.GetType();
        }

        #region reflection getters

        private PropertyInfo _GetProperty(string property)
        {
            Func<PropertyInfo> getter = () => Type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance);

            if (!UseCache)
                return getter();

            var key = new Tuple<Type, string>(Type, property);
            if (!CachedProperties.ContainsKey(key))
                CachedProperties[key] = getter();

            return CachedProperties[key];
        }

        private FieldInfo _GetField(string field)
        {
            Func<FieldInfo> getter = () => Type.GetField(field, BindingFlags.Public | BindingFlags.Instance);

            if (!UseCache)
                return getter();

            var key = new Tuple<Type, string>(Type, field);
            if (!CachedFields.ContainsKey(key))
                CachedFields[key] = getter();

            return CachedFields[key];
        }

        private MethodInfo _GetMethod(string method, Type[] argumentTypes)
        {
            Func<MethodInfo> getter = () => Type.GetMethod(method, argumentTypes);
            
            if (!UseCache)
                return getter();

            var key = new Tuple<Type, string, Type[]>(Type, method, argumentTypes);
            if (!CachedMethods.ContainsKey(key))
                CachedMethods[key] = getter();

            return CachedMethods[key];
        }

        #endregion

        #region property

        public T Property<T>(string propertyName)
        {
            var prop = _GetProperty(propertyName);
            if (prop == null)
                throw new InvalidOperationException("##");

            return (T)prop.GetValue(Item);
        }

        public void Property<T>(string propertyName, T value)
        {
            var prop = _GetProperty(propertyName);
            if (prop == null)
                throw new InvalidOperationException("##");

            prop.SetValue(Item, value);
        }

        #endregion

        #region field

        public T Field<T>(string fieldName)
        {
            var f = _GetField(fieldName);
            if (f == null)
                throw new InvalidOperationException("##");

            return (T)f.GetValue(Item);
        }

        public void Field<T>(string fieldName, T value)
        {
            var f = _GetField(fieldName);
            if (f == null)
                throw new InvalidOperationException("##");

            f.SetValue(Item, value);
        }

        #endregion

        #region Method

        public TReturn Method<TReturn>(string methodName)
        {
            return (TReturn)_CallMethod(methodName, new object[0], new Type[0]);
        }

        /// <summary>
        /// Specify arg generics explicitly if args are not exactly the same type o that the method expects (no decendent classes)
        /// </summary>
        public TReturn Method<TReturn, TArg1>(string methodName, TArg1 arg1)
        {
            return (TReturn)_CallMethod(methodName, new object[] { arg1 }, new Type[] { typeof(TArg1) });
        }

        /// <summary>
        /// Specify arg generics explicitly if args are not exactly the same type o that the method expects (no decendent classes)
        /// </summary>
        public TReturn Method<TReturn, TArg1, TArg2>(string methodName, TArg1 arg1, TArg2 arg2)
        {
            return (TReturn)_CallMethod(methodName, new object[] { arg1, arg2 }, new Type[] { typeof(TArg1), typeof(TArg2) });
        }

        /// <summary>
        /// Specify arg generics explicitly if args are not exactly the same type o that the method expects (no decendent classes)
        /// </summary>
        public TReturn Method<TReturn, TArg1, TArg2, TArg3>(string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return (TReturn)_CallMethod(methodName, new object[] { arg1, arg2, arg3 }, new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) });
        }

        private object _CallMethod(string methodName, object[] parameters, Type[] parameterTypes)
        {
            var method = _GetMethod(methodName, parameterTypes);
            if (method == null)
                throw new InvalidOperationException("##");

            return method.Invoke(Item, parameters);
        }

        #endregion

        #region Method comparer

        internal class MethodComparer : IEqualityComparer<Tuple<Type, string, Type[]>>
        {
            public bool Equals(Tuple<Type, string, Type[]> x, Tuple<Type, string, Type[]> y)
            {
                return x.Item1.Equals(y.Item1) && x.Item2.Equals(y.Item2) && AreKindOfEqual(x.Item3, y.Item3);
            }

            public int GetHashCode(Tuple<Type, string, Type[]> obj)
            {
                return (
                    obj.Item1.GetHashCode() + "_" + 
                    obj.Item2.GetHashCode() + "_" + 
                    string.Join("-", obj.Item3.Select(a => a.GetHashCode().ToString()))).GetHashCode();
            }

            public static bool AreKindOfEqual(IEnumerable<Type> item1, IEnumerable<Type> item2)
            {
                if (item1 == null && item2 == null)
                    return true;

                if (item1 == null || item2 == null || item1.Count() != item2.Count())
                    return false;

                if (item1.Equals(item2))
                    return true;

                for (var i = 0; i < item1.Count(); i++)
                    if (!item1.ElementAt(i).Equals(item2.ElementAt(i)))
                        return false;

                return true;
            }
        }

        #endregion
    }
}
