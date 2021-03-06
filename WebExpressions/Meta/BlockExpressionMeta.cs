﻿using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using WebExpressions.Enum;

namespace WebExpressions.Meta
{
    [DataContract]
    public class BlockExpressionMeta : ExpressionMeta
    {
        [DataMember]
        public ExpressionMeta[] Expressions { get; private set; }

        public BlockExpressionMeta()
            : this(null) { }

        public BlockExpressionMeta(BlockExpression expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expressions = expression.Expressions.Select(ce => CreateMeta(ce)).ToArray();
        }

        public override ExpressionWrapperType ExpressionType
        {
            get { return ExpressionWrapperType.Block; }
            protected set { /* do nothing */ }
        }
    }
}