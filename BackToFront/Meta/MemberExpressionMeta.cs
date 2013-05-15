﻿using BackToFront.Expressions;
using System.Runtime.Serialization;
using System.Linq;
using BackToFront.Enum;

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

        public MemberExpressionMeta(MemberExpressionWrapper expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expression = expression.InnerExpression.Meta;
            MemberName = expression.Expression.Member.Name;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Member; }
        }
    }
}