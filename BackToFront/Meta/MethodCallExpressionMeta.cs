using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;

namespace BackToFront.Meta
{
    [DataContract]
    public class MethodCallExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta Object { get; private set; }

        [DataMember]
        public ExpressionMeta[] Arguments { get; private set; }

        [DataMember]
        public string MethodName { get; private set; }

        [DataMember]
        public string MethodFullName { get; private set; }

        public MethodCallExpressionMeta()
            : this(null) { }

        public MethodCallExpressionMeta(MethodCallExpressionWrapper expression)
            : base(expression, Enum.ExpressionWrapperType.MethodCall)
        {
            if (expression == null)
                return;

            Object = expression.Object.Meta;
            Arguments = expression.Arguments.Select(a => a.Meta).ToArray();
            MethodName = expression.Expression.Method.Name;
            MethodFullName = expression.Expression.Method.DeclaringType.FullName + "." + expression.Expression.Method.Name;
        }
    }
}