﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;


namespace BackToFront.Utils.Expressions
{
    public class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
    {
        public ConstantExpressionWrapper(ConstantExpression expression, ReadOnlyCollection<ParameterExpression> paramaters)
            : base(expression, paramaters)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as ConstantExpressionWrapper;
            if (ex == null)
                return false;

            return Expression.Value.Equals(ex.Expression.Value);                
        }

        protected override Expression OnEvaluate(IEnumerable<Mock> mocks)
        {
            return Expression;
        }
    }
}
