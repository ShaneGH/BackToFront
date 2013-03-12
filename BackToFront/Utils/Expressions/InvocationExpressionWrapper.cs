//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//using BackToFront.Extensions.IEnumerable;

//namespace BackToFront.Utils.Expressions
//{
//    internal class InvocationExpressionWrapper : ExpressionWrapperBase<InvocationExpression>
//    {
//        private IEnumerable<ExpressionWrapperBase> _Arguments;
//        public IEnumerable<ExpressionWrapperBase> Arguments
//        {
//            get
//            {
//                return _Arguments ?? (_Arguments = Expression.Arguments.Select(a => CreateChildWrapper(a)));
//            }
//        }

//        private ExpressionWrapperBase _Lambda;
//        public ExpressionWrapperBase Lambda
//        {
//            get
//            {
//                return _Lambda ?? (_Lambda = CreateChildWrapper(Expression.Expression));
//            }
//        }

//        public InvocationExpressionWrapper(InvocationExpression expression)
//            : base(expression)
//        {
//        }

//        public override bool IsSameExpression(ExpressionWrapperBase expression)
//        {
//            var ex = expression as InvocationExpressionWrapper;
//            if (ex == null)
//                return false;

//            return Lambda.IsSameExpression(ex.Lambda) &&
//                Arguments.Count() == ex.Arguments.Count() &&
//                Arguments.All((a, i) => a.IsSameExpression(ex.Arguments.ElementAt(i)));
//        }

//        // TODO, if Expression.Method is not null
//        // TODO all node types
//        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
//        {
//            dynamic lhs = Left.Evaluate(paramaters, mocks);
//            dynamic rhs = Right.Evaluate(paramaters, mocks);

//            if (!_Evaluations.ContainsKey(Expression.NodeType))
//                throw new NotImplementedException("##" + Expression.NodeType);

//            return Evaluations[Expression.NodeType](lhs, rhs);
//        }
//    }
//}
