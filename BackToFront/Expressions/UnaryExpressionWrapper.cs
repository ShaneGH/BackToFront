using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using BackToFront.Utils;

namespace BackToFront.Expressions
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

        protected override Expression OnCompile(IEnumerable<Mock> mocks)
        {
            var result = Operand.Compile(mocks);

            return result == Operand.WrappedExpression ? Expression : Evaluations[Expression.NodeType](result, Expression);
        }
    }
}
