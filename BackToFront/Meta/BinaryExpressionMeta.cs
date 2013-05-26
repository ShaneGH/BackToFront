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
    public class BinaryExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta Left { get; private set; }

        [DataMember]
        public ExpressionMeta Right { get; private set; }

        public BinaryExpressionMeta()
            : this(null) { }

        public BinaryExpressionMeta(BinaryExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Left = CreateMeta(expression.Left);
            Right = CreateMeta(expression.Right);
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Binary; }
        }
    }
}