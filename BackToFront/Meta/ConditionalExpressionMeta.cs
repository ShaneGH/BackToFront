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

        public ConditionalExpressionMeta(ConditionalExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            IfTrue = CreateMeta(expression.IfTrue);
            IfFalse = CreateMeta(expression.IfFalse);
            Test = CreateMeta(expression.Test);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Conditional; }
            protected set { /* do nothing */ }
        }
    }
}