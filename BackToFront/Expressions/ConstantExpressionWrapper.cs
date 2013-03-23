using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using BackToFront.Utils;


namespace BackToFront.Expressions
{
    public class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
    {
        public ConstantExpressionWrapper(ConstantExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as ConstantExpressionWrapper;
            if (ex == null)
                return false;

            return Expression.Value.Equals(ex.Expression.Value);                
        }

        protected override Expression OnCompile(IEnumerable<Mock> mocks)
        {
            return Expression;
        }
    }
}
