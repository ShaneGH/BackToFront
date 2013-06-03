using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;
using System.Linq.Expressions;

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

        public MethodCallExpressionMeta(MethodCallExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Object = expression.Object != null ? CreateMeta(expression.Object) : null;
            Arguments = expression.Arguments.Select(a => CreateMeta(a)).ToArray();
            MethodName = expression.Method.Name;
            MethodFullName = expression.Method.DeclaringType.FullName + "." + expression.Method.Name;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.MethodCall; }
            protected set { /* do nothing */ }
        }
    }
}