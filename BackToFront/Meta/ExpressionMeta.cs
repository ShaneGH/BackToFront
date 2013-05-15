using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using BackToFront.Enum;
using BackToFront.Expressions;
using System.Runtime.Serialization.Json;
using System;

namespace BackToFront.Meta
{
    [DataContract]
    public abstract class ExpressionMeta
    {
        private static DataContractJsonSerializer _JsonMetaSerializer;
        private static DataContractSerializer _MetaSerializer;

        internal static readonly Type[] MetaTypes;
        static ExpressionMeta()
        {
            var type = typeof(ExpressionMeta);
            MetaTypes = type.Assembly.GetTypes().Where(t => t != type && type.IsAssignableFrom(t)).ToArray();
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

        [IgnoreDataMember]
        public abstract ExpressionWrapperType ExpressionType { get; }

        public ExpressionMeta() { }

        public ExpressionMeta(ExpressionWrapperBase expression)
            : this()
        {
            if (expression == null)
                return;

            NodeType = expression.WrappedExpression.NodeType;
        }
    }
}
