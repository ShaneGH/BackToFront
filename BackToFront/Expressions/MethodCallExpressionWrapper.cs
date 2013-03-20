using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using E = System.Linq.Expressions;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Utils;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Expressions
{
    public class MethodCallExpressionWrapper : ExpressionWrapperBase<MethodCallExpression>
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

        protected override Expression OnEvaluate(IEnumerable<Mock> mocks)
        {
            var arguments = Arguments.Select(a => a.Evaluate(mocks)).ToArray();
            var eval = Object.Evaluate(mocks);

            if (eval == Object.WrappedExpression && arguments.All((a, i) => a == Arguments.ElementAt(i).WrappedExpression))
                return Expression;

            return E.Expression.Call(eval, Expression.Method, arguments);
        }
    }
}
