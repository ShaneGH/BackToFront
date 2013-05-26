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
        }
    }
}