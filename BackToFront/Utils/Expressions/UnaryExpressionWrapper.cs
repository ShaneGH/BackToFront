using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace BackToFront.Utils.Expressions
{
    // TODO: test
    public class UnaryExpressionWrapper : ExpressionWrapperBase<UnaryExpression>
    {
        private ExpressionWrapperBase _Operand;
        public ExpressionWrapperBase Operand
        {
            get
            {
                return _Operand ?? (_Operand = CreateChildWrapper(Expression.Operand));
            }
        }

        public UnaryExpressionWrapper(UnaryExpression expression, ReadOnlyCollection<ParameterExpression> paramaters)
            : base(expression, paramaters)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as UnaryExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Method == Expression.Method && Operand.IsSameExpression(ex.Operand);
        }

        protected override Expression OnEvaluate(IEnumerable<Mock> mocks)
        {
            var result = Operand.Evaluate(mocks);

            return result == Operand.WrappedExpression ? Expression : Evaluations[Expression.NodeType](result, Expression);
        }
    }
}
