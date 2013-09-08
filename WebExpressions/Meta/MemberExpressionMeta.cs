using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public abstract class MemberExpressionMetaBase : ExpressionMeta
    {
        [DataMember]
        public string MemberName { get; private set; }

        public MemberExpressionMetaBase()
            : this(null) { }

        public MemberExpressionMetaBase(MemberExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;
            
            MemberName = expression.Member.Name;
        }

        public static MemberExpressionMetaBase CreateMemberExpressionMeta(MemberExpression expression)
        {
            return expression.Expression == null ? 
                (MemberExpressionMetaBase)new StaticMemberExpressionMeta(expression) :
                (MemberExpressionMetaBase)new MemberExpressionMeta(expression);
        }
    }

    [DataContract]
    public class MemberExpressionMeta : MemberExpressionMetaBase
    {
        [DataMember]
        public ExpressionMeta Expression { get; private set; }

        public MemberExpressionMeta()
            : this(null) { }

        public MemberExpressionMeta(MemberExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            if (expression.Expression == null)
                throw new InvalidOperationException("expression.Expression cannot be null, use StaticMemberExpressionMeta class instead");

            Expression = CreateMeta(expression.Expression);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.MemberX; }
            protected set { /* do nothing */ }
        }
    }

    [DataContract]
    public class StaticMemberExpressionMeta : MemberExpressionMetaBase
    {
        [DataMember]
        public string Class { get; private set; }

        public StaticMemberExpressionMeta()
            : this(null) { }

        public StaticMemberExpressionMeta(MemberExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            if (expression.Expression != null)
                throw new InvalidOperationException("expression.Expression must be null, use MemberExpressionMeta class instead");

            Class = expression.Member.DeclaringType.FullName;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.StaticMember; }
            protected set { /* do nothing */ }
        }
    }
}