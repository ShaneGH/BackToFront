using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;
using System;
using System.IO;

namespace BackToFront.Meta
{
    [DataContract]
    public class ConstantExpressionMeta : ExpressionMeta
    {
        // TODO: if EmitDefaultValue is removed will the json read "Value": "null"
        [DataMember(EmitDefaultValue = true)]
        public object Value { get; private set; }

        public ConstantExpressionMeta()
            : this(null) { }

        public ConstantExpressionMeta(ConstantExpressionWrapper expression)
            : base(expression)
        {
            if (expression == null)
                return;

            // TODO: need somewhere to register types for serialization
            Value = expression.Expression.Value;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Constant; }
        }
    }
}