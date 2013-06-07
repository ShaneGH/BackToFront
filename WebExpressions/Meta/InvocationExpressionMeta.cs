using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
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

        public InvocationExpressionMeta(InvocationExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expression = CreateMeta(expression.Expression);
            Arguments = expression.Arguments.Select(a => CreateMeta(a)).ToArray();
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Invocation; }
            protected set { /* do nothing */ }
        }
    }
}