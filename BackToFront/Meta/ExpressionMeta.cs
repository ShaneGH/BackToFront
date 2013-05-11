using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using BackToFront.Enum;
using BackToFront.Expressions;

namespace BackToFront.Meta
{
    [DataContract]
    public class ExpressionMeta
    {
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
