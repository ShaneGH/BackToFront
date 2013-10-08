using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace BackToFront.Web.Serialization
{

    public class MC<TMC>
    {
        public TMC Val { get; set; }
    }

    public class OneWayJsonSerializer<T>
    {
        public static class Constants
        {
            public static readonly ConstantExpression DoubleQuote = Expression.Constant(@"""");
            public static readonly ConstantExpression EscapedDoubleQuote = Expression.Constant(@"\""");
            public static readonly ConstantExpression Null = Expression.Constant(null);
            public static readonly ConstantExpression NullString = Expression.Constant("null");
            public static readonly ConstantExpression Comma = Expression.Constant(",");
            public static readonly ConstantExpression OpenParenthesis = Expression.Constant("{");
            public static readonly ConstantExpression CloseParenthesis = Expression.Constant("}");
        }

        public static class Primatives
        {
            private static readonly MethodInfo _Write = typeof(StreamWriter).GetMethod("Write", new[] { typeof(string) });
            private static readonly MethodInfo _Replace = typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) });

            public static Expression StreamWriterCall(Expression streamWriter, Expression inputString, bool escapeDoubleQuotes)
            {
                if (escapeDoubleQuotes)
                    inputString = Expression.Call(inputString, _Replace, Constants.DoubleQuote, Constants.EscapedDoubleQuote);

                return Expression.Call(streamWriter, _Write, inputString);
            }

            public static readonly ReadOnlyDictionary<Type, Func<Expression, Expression, Expression>> CommonSerializationExpressionGenerators =
                new ReadOnlyDictionary<Type, Func<Expression, Expression, Expression>>(new Dictionary<Type, Func<Expression, Expression, Expression>>
            {
                {typeof(string), (streamWriter, input) => WrapInNullReferenceCheck(Expression.Block(StreamWriterCall(streamWriter, Constants.DoubleQuote, false), StreamWriterCall(streamWriter, input, true), StreamWriterCall(streamWriter, Constants.DoubleQuote, false)), 
                    input, streamWriter)},
                {typeof(Int16), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<Int16>()), false)},
                {typeof(Int32), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<Int32>()), false)},
                {typeof(Int64), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<Int64>()), false)},
                {typeof(bool), (streamWriter, input) => StreamWriterCall(streamWriter, Expression.Call(input, _ToString<bool>()), false)}
            });

            private static MethodInfo _ToString<U>()
            {
                return typeof(U).GetMethod("ToString", new Type[0]);
            }

            public static Expression WrapInNullReferenceCheck(Expression toWrap, Expression property, Expression streamWriter)
            {
                //TODO: chek if type is nullable a little better
                return property.Type.IsValueType ?
                    toWrap :
                    Expression.IfThenElse(Expression.Equal(property, Constants.Null),
                            Primatives.StreamWriterCall(streamWriter, Constants.NullString, false),
                            toWrap);
            }
        }

        private Action<StreamWriter, T> _Worker;
        public readonly Dictionary<Type, Func<Expression, Expression, Expression>> SerializationExpressionGenerators;

        public OneWayJsonSerializer(params Type[] knownTypes)
        {
            SerializationExpressionGenerators = Primatives.CommonSerializationExpressionGenerators.ToDictionary(a => a.Key, a => a.Value);
            AddKnownTypes(new[] { typeof(T) }.Union(knownTypes ?? Enumerable.Empty<Type>()), true);
        }

        //TODO
        //public OneWayJsonSerializer(bool autoAddKnownTypes)
        //{
        //}

        public void AddKnownType(Type type, bool recompile = false)
        {
            AddKnownTypes(new[] { type }, recompile);
        }

        public void AddKnownTypes(IEnumerable<Type> types, bool recompile = false)
        {
            //TODO: need a backup in case something goes wrong in re-compile
            if (recompile)
                foreach (var key in SerializationExpressionGenerators.Keys.Where(k => !Primatives.CommonSerializationExpressionGenerators.ContainsKey(k)).ToArray())
                    SerializationExpressionGenerators[key] = null;

            foreach (var t in types.Where(k => !SerializationExpressionGenerators.ContainsKey(k)))
                SerializationExpressionGenerators.Add(t, null);

            // faster serialization
            while (SerializationExpressionGenerators.Keys.Where(k => SerializationExpressionGenerators[k] == null).Any(k => TryAddExpressionGenerator(k))) ;

            // slower serialization but can handle unknown types and circular Type references
            foreach (var key in SerializationExpressionGenerators.Keys.ToArray().Where(k => SerializationExpressionGenerators[k] == null))
                AddExpressionGenerator(key);

            if (recompile)
            {
                _Worker = CompileFor<T>();
            }
        }

        public Action<StreamWriter, U> CompileFor<U>()
        {
            var streamWriter = Expression.Parameter(typeof(StreamWriter));
            var obj = Expression.Parameter(typeof(U));
            return Expression.Lambda<Action<StreamWriter, U>>(SerializationExpressionGenerators[typeof(U)](streamWriter, obj), streamWriter, obj).Compile();
        }

        public bool TryAddExpressionGenerator(Type key)
        {
            IEnumerable<PropertyInfo> properties;
            IEnumerable<FieldInfo> fields;

            GetSerializableMembers(key, out properties, out fields);
            properties = properties.ToArray();
            fields = fields.ToArray();

            // if a converter for this field/property has not been populated yet
            if (properties.Select(p => p.PropertyType).Union(fields.Select(f => f.FieldType))
                .Any(t => !SerializationExpressionGenerators.ContainsKey(t) || SerializationExpressionGenerators[t] == null))
                return false;

            _AddExpressionGenerator(key, properties, fields);

            return true;
        }

        public void AddExpressionGenerator(Type key)
        {
            IEnumerable<PropertyInfo> properties;
            IEnumerable<FieldInfo> fields;

            GetSerializableMembers(key, out properties, out fields);
            properties = properties.ToArray();
            fields = fields.ToArray();

            _AddExpressionGenerator(key, properties, fields);
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

        private BlockExpression GenerateLazyLoadedSerializationExpression(Type propertyType, Expression streamWriter, Expression property)
        {
            var containsKey = typeof(Dictionary<Type, Func<Expression, Expression, Expression>>).GetMethod("ContainsKey");
            var compileFor = this.GetType().GetMethod("CompileFor").MakeGenericMethod(propertyType);

            var cache = Expression.Property(Expression.Constant(Activator.CreateInstance(typeof(MC<>).MakeGenericType(compileFor.ReturnType))), "Val");
            return Expression.Block(
                // if cache.Val == null
                Expression.IfThen(Expression.Equal(cache, Constants.Null),
                // if SerializationExpressionGenerators.ContainsKey(propertyType)
                    Expression.IfThenElse(Expression.Call(Expression.Constant(SerializationExpressionGenerators), containsKey, Expression.Constant(propertyType)),
                // cache.Val = this.CompileFor<TPropertyType>()
                        Expression.Assign(cache, Expression.Call(Expression.Constant(this), compileFor)),
                // else throw exception
                        Expression.Throw(Expression.Constant(new InvalidOperationException("##" + "Add type to known types"))))),
                // cache.Val(streamWriter, property)
                Expression.Invoke(cache, streamWriter, property));
        }

        public void _AddExpressionGenerator(Type key, IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields)
        {
            // test all properties/fields are from the correct class
            if (properties.Cast<MemberInfo>().Union(fields).Any(p => p.DeclaringType != key))
                throw new InvalidOperationException("##");

            // final serialzer
            Func<Expression, Expression, Expression> expressionGenerator = (streamWriter, obj) =>
            {
                var serializers = properties.Select(p => p.Name).Union(fields.Select(p => p.Name)).Select(i =>
                {
                    var property = Expression.PropertyOrField(obj, i);
                    Expression writer = null;
                    if (SerializationExpressionGenerators.ContainsKey(property.Type))
                        writer = SerializationExpressionGenerators[property.Type](streamWriter, property);
                    else
                        writer = GenerateLazyLoadedSerializationExpression(property.Type, streamWriter, property);

                    return new KeyValuePair<string, Expression>(i,
                      Expression.Block(Primatives.StreamWriterCall(streamWriter, Expression.Constant("\"" + i + "\":"), false), writer));
                }).OrderBy(n => n.Key).Select(n => n.Value).ToList();

                // add commas
                for (var i = 1; i < serializers.Count; i += 2)
                    serializers.Insert(i, Expression.Block(Primatives.StreamWriterCall(streamWriter, Constants.Comma, false)));

                // add beginning and end parenthesis
                serializers.Insert(0, Primatives.StreamWriterCall(streamWriter, Constants.OpenParenthesis, false));
                serializers.Add(Primatives.StreamWriterCall(streamWriter, Constants.CloseParenthesis, false));

                return Primatives.WrapInNullReferenceCheck(Expression.Block(serializers), obj, streamWriter);
            };

            if (SerializationExpressionGenerators.ContainsKey(key))
                SerializationExpressionGenerators[key] = expressionGenerator;
            else
                SerializationExpressionGenerators.Add(key, expressionGenerator);
        }

        public void WriteObject(StreamWriter stream, T toWrite)
        {
            _Worker(stream, toWrite);
        }
    }
}
