using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using E = System.Linq.Expressions;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Utilities;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Expressions
{
    public class MethodCallExpressionWrapper : ExpressionWrapperBase<MethodCallExpression>, ILinearExpression
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

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as MethodCallExpression;
            if (ex == null)
                return false;

            return Expression.Method.GetBaseDefinition() == ex.Method.GetBaseDefinition() &&
                Object.IsSameExpression(ex.Object) &&                
                Arguments.Count() == ex.Arguments.Count() &&
                Arguments.All((a, b) => a.IsSameExpression(ex.Arguments.ElementAt(b)));
        }

        protected override Expression CompileInnerExpression(ISwapPropVisitor mocks)
        {
            var arguments = Arguments.Select(a => a.Compile(mocks)).ToArray();
            var eval = Object.Compile(mocks);

            if (eval == Object.WrappedExpression && arguments.All((a, i) => a == Arguments.ElementAt(i).WrappedExpression))
                return Expression;

            return E.Expression.Call(eval, Expression.Method, arguments);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return new[] { Object.GetMembersForParameter(parameter).Each(i => i.NextItem = new MemberChainItem(Expression.Method)) }
                .Concat(Arguments.Select(a => a.GetMembersForParameter(parameter)))
                .Aggregate();
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Object.UnorderedParameters.Union(Arguments.Select(a => a.UnorderedParameters).Aggregate());
            }
        }

        public ExpressionWrapperBase Root
        {
            get { return Object; }
        }

        public ExpressionWrapperBase WithAlternateRoot<TEntity, TChild>(Expression root, Expression<Func<TEntity, TChild>> child)
        {
            return new MethodCallExpressionWrapper(E.Expression.Call(root, Expression.Method, Arguments.Select(a => 
            {
                if (a.UnorderedParameters.Count() > 0)
                    return a.ForChildExpression(child, root);
                else
                    return a.WrappedExpression;
            })));
        }

        private ExpressionElementMeta _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ExpressionElementMeta(Expression.Method.Name, Arguments.Select(a => a.Meta).ToArray(), ExpressionWrapperType.MethodCall, Expression.Type, Object.Meta));                
            }
        }
    }
}
