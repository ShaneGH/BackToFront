using BackToFront.Enum;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace BackToFront.Meta
{
    [DataContract]
    public class NewExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta[] Arguments { get; private set; }

        [DataMember]
        public string[] Members { get; private set; }

        [DataMember]
        public string Type { get; private set; }

        public NewExpressionMeta()
            : this(null) { }

        public NewExpressionMeta(NewExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Type = expression.Constructor.DeclaringType.FullName;
            Arguments = expression.Arguments.Select(a => CreateMeta(a)).ToArray();
            Members = (expression.Members ?? Enumerable.Empty<MemberInfo>()).Select(m => m.Name).ToArray();
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.New; }
        }
    }
}