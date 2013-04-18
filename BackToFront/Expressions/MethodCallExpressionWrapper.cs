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
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as MethodCallExpressionWrapper;
            if (ex == null)
                return false;
            
            return Expression.Method == ex.Expression.Method &&
                Object.IsSameExpression(ex.Object) &&                
                Arguments.Count() == ex.Arguments.Count() &&
                Arguments.All((a, b) => a.IsSameExpression(ex.Arguments.ElementAt(b)));
        }

        protected override Expression CompileInnerExpression(Mocks mocks)
        {
            var arguments = Arguments.Select(a => a.Compile(mocks)).ToArray();
            var eval = Object.Compile(mocks);

            if (eval == Object.WrappedExpression && arguments.All((a, i) => a == Arguments.ElementAt(i).WrappedExpression))
                return Expression;

            return E.Expression.Call(eval, Expression.Method, arguments);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            var items =
                new[] { Object.GetMembersForParameter(parameter).Each(i => i.NextItem = new MemberChainItem(Expression.Method)) }
                .Concat(Arguments.Select(a => a.GetMembersForParameter(parameter)))
                .Aggregate();

            foreach (var item in items)
                yield return item;
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Object.UnorderedParameters.Union(Arguments.Select(a => a.UnorderedParameters).Aggregate());
            }
        }
    }
}
