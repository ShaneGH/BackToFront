using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BackToFront.Enum;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BackToFront.Expressions;

namespace BackToFront.Meta
{
    [DataContract]
    public class UnaryExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta Operand { get; private set; }

        public UnaryExpressionMeta()
            : this(null) { }

        public UnaryExpressionMeta(UnaryExpressionWrapper expression)
            : base(expression, ExpressionWrapperType.Member)
        {
            if (expression == null)
                return;

            Operand = expression.Operand.Meta;
        }
    }
}