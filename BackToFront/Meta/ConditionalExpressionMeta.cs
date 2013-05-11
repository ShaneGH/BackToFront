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
    public class ConditionalExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta IfTrue { get; private set; }

        [DataMember]
        public ExpressionMeta IfFalse { get; private set; }

        [DataMember]
        public ExpressionMeta Test { get; private set; }

        public ConditionalExpressionMeta()
            : this(null) { }

        public ConditionalExpressionMeta(ConditionalExpressionWrapper expression)
            : base(expression, ExpressionWrapperType.Conditional)
        {
            if (expression == null)
                return;

            IfTrue = expression.IfTrue.Meta;
            IfFalse = expression.IfFalse.Meta;
            Test = expression.Test.Meta;
        }
    }
}