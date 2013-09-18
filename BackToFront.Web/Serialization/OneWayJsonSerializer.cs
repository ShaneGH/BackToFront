using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace BackToFront.Web.Serialization
{
    public class OneWayJsonSerializer<T>
    {
        public static class Primatives
        {
            private static readonly MethodInfo _Write = typeof(StreamWriter).GetMethod("Write", new[] { typeof(string) });
            private static readonly MethodInfo _Replace = typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) });

            public static Expression StreamWriterCall(Expression streamWriter, Expression inputString, bool escapeDoubleQuotes)
            {
                if (escapeDoubleQuotes)
                    inputString = Expression.Call(inputString, _Replace, Expression.Constant(@""""), Expression.Constant(@"\"""));

                return Expression.Call(streamWriter, _Write, inputString);
            }

            public static readonly ReadOnlyDictionary<Type, Func<Expression, Expression, Expression>> Converters =
                new ReadOnlyDictionary<Type, Func<Expression, Expression, Expression>>(new Dictionary<Type, Func<Expression, Expression, Expression>>
            {
                {typeof(string), (streamWriter, input) => Expression.Block(StreamWriterCall(streamWriter, Expression.Constant("\""), false), StreamWriterCall(streamWriter, input, true), StreamWriterCall(streamWriter, Expression.Constant("\""), false))},
                {typeof(Int16), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<Int16>()), false)},
                {typeof(Int32), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<Int32>()), false)},
                {typeof(Int64), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<Int64>()), false)},
                {typeof(bool), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<bool>()), false)}
            });

            private static MethodInfo _ToString<U>()
            {
                return typeof(U).GetMethod("ToString", new Type[0]);
            }
        }

        private Action<StreamWriter, T> _Worker;
        public readonly Dictionary<Type, Func<Expression, Expression, Expression>> AllTypes;

        public OneWayJsonSerializer(params Type[] knownTypes)
        {
            AllTypes = Primatives.Converters.ToDictionary(a => a.Key, a => a.Value);
            AddKnownTypes((knownTypes ?? Enumerable.Empty<Type>()).Union(new[] { typeof(T) }), true);
        }

        public void AddKnownTypes(IEnumerable<Type> types, bool recompile = false)
        {
            //TODO: need a backup in case something goes wrong in re-compile
            if (recompile)
                foreach (var key in AllTypes.Keys.Where(k => !Primatives.Converters.ContainsKey(k)))
                    AllTypes[key] = null;

            foreach (var t in types.Where(k => !AllTypes.ContainsKey(k)))
                AllTypes.Add(t, null);

            // faster serialization
            while (AllTypes.Keys.Where(k => AllTypes[k] == null).Any(k => TryAddConverter(k))) ;

            // slower serialization but can handler unknown types and circular Type references
            foreach (var key in AllTypes.Keys.ToArray().Where(k => AllTypes[k] == null))
                AddConverter(key);

            if (recompile)
            {
                var streamWriter = Expression.Parameter(typeof(StreamWriter));
                var obj = Expression.Parameter(typeof(T));
                var ooo = AllTypes[typeof(T)](streamWriter, obj);
                _Worker = Expression.Lambda<Action<StreamWriter, T>>(AllTypes[typeof(T)](streamWriter, obj), streamWriter, obj).Compile();
            }
        }

        public bool TryAddConverter(Type key)
        {
            IEnumerable<PropertyInfo> properties;
            IEnumerable<FieldInfo> fields;

            GetSerializableMembers(key, out properties, out fields);
            properties = properties.ToArray();
            fields = fields.ToArray();

            // if a converter for this field/property has not been populated yet
            if (properties.Select(p => p.PropertyType).Union(fields.Select(f => f.FieldType))
                .Any(t => !AllTypes.ContainsKey(t) || AllTypes[t] == null))
                return false;

            _AddConverter(key, properties, fields);

            return true;
        }

        public void AddConverter(Type key)
        {
            IEnumerable<PropertyInfo> properties;
            IEnumerable<FieldInfo> fields;

            GetSerializableMembers(key, out properties, out fields);
            properties = properties.ToArray();
            fields = fields.ToArray();

            Func<Func<Expression, Expression, Expression>> getConverter = () => 
            {
                if (!AllTypes.ContainsKey(key))
                    throw new InvalidOperationException("##" + "Add type to known types");

                return AllTypes[key]; 
            };

            _AddConverter(key, properties, fields);
        }

        public void GetSerializableMembers(Type type, out IEnumerable<PropertyInfo> properties, out IEnumerable<FieldInfo> fields)
        {
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            fields = Enumerable.Empty<FieldInfo>();

            if (type.GetCustomAttribute<DataContractAttribute>() != null)
            {
                properties = properties.Where(m => m.GetCustomAttribute<DataMemberAttribute>() != null);
                fields = type.GetFields().Where(m => m.GetCustomAttribute<DataMemberAttribute>() != null);
            }
        }

        public void _AddConverter(Type key, IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields)
        {
            if (properties.Cast<MemberInfo>().Union(fields).Any(p => p.DeclaringType != key))
                throw new InvalidOperationException("##");

            Func<Type, Expression, Expression, Expression> getSerializer = (t, streamWriter, propertyOrField) => 
            {
                if (AllTypes.ContainsKey(t) && AllTypes[t] != null)
                    return AllTypes[t](streamWriter, propertyOrField);
                else
                {
                    Func<Func<Expression, Expression, Expression>> getConverter = () =>
                    {
                        if (!AllTypes.ContainsKey(key))
                            throw new InvalidOperationException("##" + "Add type to known types");

                        return AllTypes[key];
                    };

                    // TODO: make sure nothing is compiled at serialize time (especially anything coming out of the builders in the dictionary)
                    return Expression.Invoke(
                        Expression.Invoke(Expression.Constant(getConverter)),
                        Expression.Constant(streamWriter),
                        Expression.Constant(propertyOrField));
                }
            };

            Func<Expression, Expression, Expression> val = (streamWriter, obj) =>
            {
                var propertySerializers = properties.Select(i =>
                {
                    return new KeyValuePair<string, BlockExpression>(i.Name,
                        Expression.Block(
                        Primatives.StreamWriterCall(streamWriter, Expression.Constant("\"" + i.Name + "\":"), false),
                        getSerializer(i.PropertyType, streamWriter, Expression.Property(obj, i))));
                });

                var fieldSerializers = fields.Select(i =>
                {
                    return new KeyValuePair<string, BlockExpression>(i.Name,
                        Expression.Block(
                        Primatives.StreamWriterCall(streamWriter, Expression.Constant("\"" + i.Name + "\":"), false),
                        getSerializer(i.FieldType, streamWriter, Expression.Field(obj, i))));
                });

                var all = propertySerializers.Union(fieldSerializers).OrderBy(n => n.Key).Select(n => n.Value).ToList();
                for (var i = 1; i < all.Count; i += 2)
                    all.Insert(1, Expression.Block(Primatives.StreamWriterCall(streamWriter, Expression.Constant(","), false)));

                return Expression.Block(
                    new[] { Primatives.StreamWriterCall(streamWriter, Expression.Constant("{"), false) }
                    .Union(all)
                    .Union(new[] { Primatives.StreamWriterCall(streamWriter, Expression.Constant("}"), false) }));
            };

            if (AllTypes.ContainsKey(key))
                AllTypes[key] = val;
            else
                AllTypes.Add(key, val);
        }

        public void WriteObject(StreamWriter stream, T toWrite)
        {
            _Worker(stream, toWrite);
        }
    }
}
