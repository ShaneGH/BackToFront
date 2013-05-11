using BackToFront.Expressions;
using System.Runtime.Serialization;

namespace BackToFront.Meta
{
    [DataContract]
    public class ParameterExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public string Name { get; private set; }

        public ParameterExpressionMeta()
            : this(null) { }

        public ParameterExpressionMeta(ParameterExpressionWrapper expression)
            : base(expression, Enum.ExpressionWrapperType.Parameter)
        {
            if (expression == null)
                return;

            Name = expression.Expression.Name;
        }
    }
}