using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using BackToFront.Enum;
using BackToFront.Expressions;
using System.Runtime.Serialization.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace BackToFront.Meta
{
    [DataContract]
    public abstract class ExpressionMeta : IMeta
    {
        public static readonly ReadOnlyDictionary<Type, Func<Expression, ExpressionMeta>> Constructors;

        private static DataContractJsonSerializer _JsonMetaSerializer;
        private static DataContractSerializer _MetaSerializer;

        internal static readonly Type[] MetaTypes;
        static ExpressionMeta()
        {
            var type = typeof(ExpressionMeta);
            MetaTypes = type.Assembly.GetTypes().Where(t => t != type && type.IsAssignableFrom(t)).ToArray();

            var constructors = new Dictionary<Type, Func<Expression, ExpressionMeta>>();

            constructors[typeof(BinaryExpression)] = expression => new BinaryExpressionMeta(expression as BinaryExpression);
            constructors[typeof(ConstantExpression)] = expression => new ConstantExpressionMeta(expression as ConstantExpression);
            constructors[typeof(MethodCallExpression)] = expression => new MethodCallExpressionMeta(expression as MethodCallExpression);
            constructors[typeof(UnaryExpression)] = expression => new UnaryExpressionMeta(expression as UnaryExpression);
            constructors[typeof(ParameterExpression)] = expression => new ParameterExpressionMeta(expression as ParameterExpression);
            constructors[typeof(MemberExpression)] = expression => new MemberExpressionMeta(expression as MemberExpression);
            constructors[typeof(BlockExpression)] = expression => new BlockExpressionMeta(expression as BlockExpression);
            constructors[typeof(ConditionalExpression)] = expression => new ConditionalExpressionMeta(expression as ConditionalExpression);
            constructors[typeof(DefaultExpression)] = expression => new DefaultExpressionMeta(expression as DefaultExpression);
            constructors[typeof(InvocationExpression)] = expression => new InvocationExpressionMeta(expression as InvocationExpression);

            Constructors = new ReadOnlyDictionary<Type, Func<Expression, ExpressionMeta>>(constructors);
        }

        /// <summary>
        /// A serializer for all Expression meta types in this assembly
        /// </summary>
        public static DataContractSerializer MetaSerializer
        {
            get
            {
                if (_MetaSerializer == null)
                    _MetaSerializer = new DataContractSerializer(typeof(ExpressionMeta), MetaTypes);

                return _MetaSerializer;
            }
        }

        /// <summary>
        /// A serializer for all Expression meta types in this assembly
        /// </summary>
        public static DataContractJsonSerializer JsonMetaSerializer
        {
            get
            {
                if (_JsonMetaSerializer == null)
                    //TODO: can't seem to get rid of the __type with emitType = true (or can I)
                    _JsonMetaSerializer = new DataContractJsonSerializer(typeof(ExpressionMeta), new DataContractJsonSerializerSettings { KnownTypes = MetaTypes, EmitTypeInformation = EmitTypeInformation.Always });

                return _JsonMetaSerializer;
            }
        }

        [DataMember]
        public ExpressionType NodeType { get; private set; }

        [DataMember]
        public abstract ExpressionWrapperType ExpressionType { get; }

        public ExpressionMeta() { }

        public ExpressionMeta(Expression expression)
            : this()
        {
            if (expression == null)
                return;

            NodeType = expression.NodeType;
        }

        public static ExpressionMeta CreateMeta(Expression expression)
        {
            var type = expression.GetType();

            while (type != typeof(Expression))
            {
                if (Constructors.ContainsKey(type))
                    break;
                else
                    type = type.BaseType;
            }

            if (type == typeof(Expression))
                throw new InvalidOperationException("##" + expression.GetType().ToString());

            return Constructors[type](expression);
        }
    }
}
