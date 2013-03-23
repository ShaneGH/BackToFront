using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Utils;


namespace BackToFront.Expressions
{
    public class BinaryExpressionWrapper : ExpressionWrapperBase<BinaryExpression>
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
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as BinaryExpressionWrapper;
            if (ex == null)
                return false;

            return Expression.NodeType == ex.Expression.NodeType &&
                Expression.Method == ex.Expression.Method &&
                Left.IsSameExpression(ex.Left) &&
                Right.IsSameExpression(ex.Right);                
        }

        protected override Expression CompileInnerExpression(IEnumerable<Mock> mocks)
        {
            Expression lhs = Left.Compile(mocks);
            Expression rhs = Right.Compile(mocks);

            if (lhs == Left.WrappedExpression && rhs == Right.WrappedExpression)
                return Expression;

            if (!Evaluations.ContainsKey(Expression.NodeType))
                throw new NotImplementedException("##" + Expression.NodeType);

            return Evaluations[Expression.NodeType](lhs, rhs);
        }
    }
}
