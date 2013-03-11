using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace BackToFront.Utils.Expressions
{
    internal class BinaryExpressionWrapper : ExpressionWrapperBase<BinaryExpression>
    {
        internal static readonly ReadonlyDictionary<ExpressionType, Func<dynamic, dynamic, dynamic>> Evaluations;
        private static readonly Dictionary<ExpressionType, Func<dynamic, dynamic, dynamic>> _Evaluations = new Dictionary<ExpressionType, Func<dynamic, dynamic, dynamic>>();

        static BinaryExpressionWrapper()
        {
            _Evaluations[ExpressionType.AndAlso] = (lhs, rhs) => lhs && rhs;
            _Evaluations[ExpressionType.OrElse] = (lhs, rhs) => lhs || rhs;
            _Evaluations[ExpressionType.Equal] = (lhs, rhs) => lhs == rhs;
            _Evaluations[ExpressionType.NotEqual] = (lhs, rhs) => lhs != rhs;
            _Evaluations[ExpressionType.Add] = (lhs, rhs) => lhs + rhs;
            _Evaluations[ExpressionType.Subtract] = (lhs, rhs) => lhs - rhs;

            Evaluations = new ReadonlyDictionary<ExpressionType, Func<dynamic, dynamic, dynamic>>(_Evaluations);
        }

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
        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
        {
            dynamic lhs = Left.Evaluate(paramaters, mocks);
            dynamic rhs = Right.Evaluate(paramaters, mocks);

            if (!_Evaluations.ContainsKey(Expression.NodeType))
                throw new NotImplementedException("##");

            return Evaluations[Expression.NodeType](lhs, rhs);
        }
    }
}
