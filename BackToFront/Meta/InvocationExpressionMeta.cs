using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;

namespace BackToFront.Meta
{
    [DataContract]
    public class InvocationExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta Expression { get; private set; }

        [DataMember]
        public ExpressionMeta[] Arguments { get; private set; }

        public InvocationExpressionMeta()
            : this(null) { }

        public InvocationExpressionMeta(InvocationExpressionWrapper expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expression = expression.InnerExpression.Meta;
            Arguments = expression.Arguments.Select(a => a.Meta).ToArray();
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Invocation; }
        }
    }
}