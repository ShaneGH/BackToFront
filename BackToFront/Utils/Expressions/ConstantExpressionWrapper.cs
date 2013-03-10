using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace BackToFront.Utils.Expressions
{
    internal class ConstantExpressionWrapper : ExpressionWrapperBase<ConstantExpression>
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

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
        {
            return Expression.Value;
        }
    }
}
