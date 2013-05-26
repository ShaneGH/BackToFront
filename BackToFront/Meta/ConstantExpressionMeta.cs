using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;
using System;
using System.IO;
using System.Linq.Expressions;

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

        public ConstantExpressionMeta(ConstantExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            // TODO: need somewhere to register types for serialization
            Value = expression.Value;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Constant; }
        }
    }
}