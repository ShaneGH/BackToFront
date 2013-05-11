using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;

namespace BackToFront.Meta
{
    [DataContract]
    public class ConstantExpressionMeta : ExpressionMeta
    {
        // TODO: this
        //[DataMember]
        public object Value { get; private set; }

        public ConstantExpressionMeta()
            : this(null) { }

        public ConstantExpressionMeta(ConstantExpressionWrapper expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Value = expression.Expression.Value;
        }
    }
}