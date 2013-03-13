using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace BackToFront.Utils.Expressions
{
    internal class BinaryExpressionWrapper : ExpressionWrapperBase<BinaryExpression>
    {
        private ExpressionWrapperBase _Left;
        private ExpressionWrapperBase Left
        {
            get
            {
                return _Left ?? (_Left = CreateChildWrapper(Expression.Left));
            }
        }

        private ExpressionWrapperBase _Right;
        private ExpressionWrapperBase Right
        {
            get
            {
                return _Right ?? (_Right = CreateChildWrapper(Expression.Right));
            }
        }

        public BinaryExpressionWrapper(BinaryExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as BinaryExpressionWrapper;
            if (ex == null)
                return false;

            return Expression.NodeType == ex.Expression.NodeType &&
                Expression.Method == ex.Expression.Method &&
                Left.IsSameExpression(ex.Left) &&
                Right.IsSameExpression(ex.Right);
                
        }

        // TODO, if Expression.Method is not null
        // TODO all node types
        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks)
        {
            dynamic lhs = Left.Evaluate(paramaters, mocks);
            dynamic rhs = Right.Evaluate(paramaters, mocks);

            if (!Evaluations.ContainsKey(Expression.NodeType))
                throw new NotImplementedException("##" + Expression.NodeType);

            return Evaluations[Expression.NodeType](lhs, rhs);
        }
    }
}
