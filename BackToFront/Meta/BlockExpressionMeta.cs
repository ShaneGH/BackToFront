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

        public BlockExpressionMeta(BlockExpressionWrapper expression)
            : base(expression)
        {
            if (expression == null)
                return;

            Expressions = expression.ChildExpressions.Select(ce => ce.Meta).ToArray();
        }
    }
}