using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace BackToFront.Utils.Expressions
{
    // TODO: test
    internal class UnaryExpressionWrapper : ExpressionWrapperBase<UnaryExpression>
    {
        private ExpressionWrapperBase _Operand;
        public ExpressionWrapperBase Operand
        {
            get
            {
                return _Operand ?? (_Operand = CreateChildWrapper(Expression.Operand));
            }
        }

        public UnaryExpressionWrapper(UnaryExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as UnaryExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Method == Expression.Method && Operand.IsSameExpression(ex.Operand);
        }

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
        {
            return Operand.Evaluate(paramaters, mocks);
        }
    }
}
