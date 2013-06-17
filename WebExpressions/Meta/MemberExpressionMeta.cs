﻿using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
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

        public MemberExpressionMeta(MemberExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expression = expression.Expression != null ? CreateMeta(expression.Expression) : null;
            MemberName = expression.Member.Name;
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Member; }
            protected set { /* do nothing */ }
        }
    }
}