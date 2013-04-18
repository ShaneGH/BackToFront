﻿using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Utils;
using System.Linq.Expressions;

namespace BackToFront.Dependency
{
    public class RuleDependency
    {
        public readonly string Name;
        public readonly object Value;

        public RuleDependency(string name, object value)
        {
            Name = name;
            Value = value;
        }

        internal Mock ToMock()
        {
            return new Mock(new ConstantExpressionWrapper(Expression.Constant(this)), Value, Value.GetType(), MockBehavior.MockOnly);
        }
    }
}