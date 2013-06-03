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

        public UnaryExpressionMeta(UnaryExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Operand = CreateMeta(expression.Operand);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Unary; }
            protected set { /* do nothing */ }
        }
    }
}