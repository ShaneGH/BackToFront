using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using BackToFront.Enum;
using BackToFront.Expressions;
using System.Runtime.Serialization.Json;

namespace BackToFront.Meta
{
    [DataContract]
    public class ExpressionMeta
    {
        private static DataContractJsonSerializer _MetaJsonSerializer;
        private static DataContractSerializer _MetaSerializer;

        /// <summary>
        /// A serializer for all Expression meta types in this assembly
        /// </summary>
        public static DataContractSerializer MetaSerializer
        {
            get
            {
                if (_MetaSerializer == null)
                {
                    var type = typeof(ExpressionMeta);
                    var types = type.Assembly.GetTypes().Where(t => t != type && type.IsAssignableFrom(t));
                    _MetaSerializer = new DataContractSerializer(type, types);
                }

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
                if (_MetaJsonSerializer == null)
                {
                    var type = typeof(ExpressionMeta);
                    var types = type.Assembly.GetTypes().Where(t => t != type && type.IsAssignableFrom(t));
                    //TODO: can't seem to get rid of the __type with emitType = true
                    _MetaJsonSerializer = new DataContractJsonSerializer(type, types, int.MaxValue, false, null, true);
                }

                return _MetaJsonSerializer;
            }
        }

        [DataMember]
        public ExpressionType NodeType { get; private set; }

        [DataMember]
        public ExpressionWrapperType ExpressionType { get; private set; }

        public ExpressionMeta() { }

        public ExpressionMeta(ExpressionWrapperBase expression, ExpressionWrapperType expressionType)
            : this()
        {
            if (expression == null)
                return;

            NodeType = expression.WrappedExpression.NodeType;
            ExpressionType = expressionType;
        }
    }
}
