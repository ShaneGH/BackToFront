using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;
using System.Linq.Expressions;

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
        }
    }
}