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
    public class DefaultExpressionMeta : ExpressionMeta
    {
        public DefaultExpressionMeta()
            : this(null) { }

        public DefaultExpressionMeta(DefaultExpressionWrapper expression)
            : base(expression) { }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Default; }
        }
    }
}