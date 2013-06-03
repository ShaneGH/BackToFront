using System.Linq.Expressions;
using System.Runtime.Serialization;
using BackToFront.Enum;

namespace BackToFront.Meta
{
    [DataContract]
    public class ParameterExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public string Name { get; private set; }

        public ParameterExpressionMeta()
            : this(null) { }

        public ParameterExpressionMeta(ParameterExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Name = expression.Name;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Parameter; }
            protected set { /* do nothing */ }
        }
    }
}