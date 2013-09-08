using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public abstract class MethodCallExpressionMetaBase : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta[] Arguments { get; private set; }

        [DataMember]
        public string MethodName { get; private set; }

        public MethodCallExpressionMetaBase()
            : this(null) { }

        public MethodCallExpressionMetaBase(MethodCallExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Arguments = expression.Arguments.Select(a => CreateMeta(a)).ToArray();
            MethodName = expression.Method.Name;
        }

        public static MethodCallExpressionMetaBase CreateMethodCallExpressionMeta(MethodCallExpression expression)
        {
            return expression.Object == null ?
                (MethodCallExpressionMetaBase)new StaticMethodCallExpressionMeta(expression) :
                (MethodCallExpressionMetaBase)new MethodCallExpressionMeta(expression);
        }
    }

    [DataContract]
    public class MethodCallExpressionMeta : MethodCallExpressionMetaBase
    {
        [DataMember]
        public ExpressionMeta Object { get; private set; }

        public MethodCallExpressionMeta()
            : this(null) { }

        public MethodCallExpressionMeta(MethodCallExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            if (expression.Object == null)
                throw new InvalidOperationException("##");

            Object = CreateMeta(expression.Object);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.MethodCall; }
            protected set { /* do nothing */ }
        }
    }

    [DataContract]
    public class StaticMethodCallExpressionMeta : MethodCallExpressionMetaBase
    {
        [DataMember]
        public string Class { get; private set; }

        public StaticMethodCallExpressionMeta()
            : this(null) { }

        public StaticMethodCallExpressionMeta(MethodCallExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            if (expression.Object != null)
                throw new InvalidOperationException("##");

            Class = expression.Method.DeclaringType.FullName;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.StaticMethodCall; }
            protected set { /* do nothing */ }
        }
    }
}