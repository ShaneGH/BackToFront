using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Utils.Expressions
{
    internal class MethodCallExpressionWrapper : ExpressionWrapperBase<MethodCallExpression>
    {
        private ExpressionWrapperBase _Object;
        public ExpressionWrapperBase Object
        {
            get
            {
                return _Object ?? (_Object = CreateChildWrapper(Expression.Object));
            }
        }

        private IEnumerable<ExpressionWrapperBase> _Arguments;
        public IEnumerable<ExpressionWrapperBase> Arguments
        {
            get
            {
                return _Arguments ?? (_Arguments = Expression.Arguments.Select(a => CreateChildWrapper(a)).ToArray());
            }
        }    

        public MethodCallExpressionWrapper(MethodCallExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            var ex = expression as MethodCallExpressionWrapper;
            if (ex == null)
                return false;
            
            return Expression.Method == ex.Expression.Method &&
                Object.IsSameExpression(ex.Object) &&                
                Arguments.Count() == ex.Arguments.Count() &&
                Arguments.All((a, b) => a.IsSameExpression(ex.Arguments.ElementAt(b)));
        }

        protected override object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks)
        {
            var arguments = Arguments.Select(a => a.Evaluate(paramaters, mocks)).ToArray();
            var eval = Object.Evaluate(paramaters, mocks);
            return Expression.Method.Invoke(eval, arguments);
        }
    }
}
