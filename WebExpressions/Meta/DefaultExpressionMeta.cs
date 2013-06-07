using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public class DefaultExpressionMeta : ExpressionMeta
    {
        public DefaultExpressionMeta()
            : this(null) { }

        public DefaultExpressionMeta(DefaultExpression expression)
            : base(expression) { }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Default; }
            protected set { /* do nothing */ }
        }
    }
}