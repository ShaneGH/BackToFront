using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Utils.Expressions
{
    internal class ParameterExpressionWrapper : ExpressionWrapperBase<ParameterExpression>
    {
        public ParameterExpressionWrapper(ParameterExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as ParameterExpressionWrapper;
            if (ex == null)
                return false;

            return Index >= 0 && Index == ex.Index;
        }

        private int Index
        {
            get
            {
                return Parameters.IndexOf(Expression);
            }
        }

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
        {
            return paramaters.ElementAt(Index);
        }
    }
}
