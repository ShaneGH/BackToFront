using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public class ConstantExpressionMeta : ExpressionMeta
    {
        // TODO: if EmitDefaultValue is removed will the json read "Value": "null"
        [DataMember(EmitDefaultValue = true)]
        public object Value { get; private set; }

        public ConstantExpressionMeta()
            : this(null) { }

        public ConstantExpressionMeta(ConstantExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            // TODO: need somewhere to register types for serialization
            Value = expression.Value;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Constant; }
            protected set { /* do nothing */ }
        }
    }
}