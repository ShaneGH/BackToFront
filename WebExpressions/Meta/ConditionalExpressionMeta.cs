using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public class ConditionalExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta IfTrue { get; private set; }

        [DataMember]
        public ExpressionMeta IfFalse { get; private set; }

        [DataMember]
        public ExpressionMeta Test { get; private set; }

        public ConditionalExpressionMeta()
            : this(null) { }

        public ConditionalExpressionMeta(ConditionalExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            IfTrue = CreateMeta(expression.IfTrue);
            IfFalse = CreateMeta(expression.IfFalse);
            Test = CreateMeta(expression.Test);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Conditional; }
            protected set { /* do nothing */ }
        }
    }
}