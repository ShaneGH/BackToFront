using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;
using System.Linq.Expressions;

namespace BackToFront.Meta
{
    [DataContract]
    public class MemberExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta Expression { get; private set; }

        [DataMember]
        public string MemberName { get; private set; }

        public MemberExpressionMeta()
            : this(null) { }

        public MemberExpressionMeta(MemberExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expression = expression.Expression != null ? CreateMeta(expression.Expression) : null;
            MemberName = expression.Member.Name;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Member; }
        }
    }
}