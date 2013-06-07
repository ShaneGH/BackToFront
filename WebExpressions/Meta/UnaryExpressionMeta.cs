using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public class UnaryExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta Operand { get; private set; }

        public UnaryExpressionMeta()
            : this(null) { }

        public UnaryExpressionMeta(UnaryExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Operand = CreateMeta(expression.Operand);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Unary; }
            protected set { /* do nothing */ }
        }
    }
}