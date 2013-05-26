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

        protected override bool IsSameExpression(MethodCallExpression expression)
        {
            return Expression.Method.GetBaseDefinition() == expression.Method.GetBaseDefinition() &&
                Object.IsSameExpression(expression.Object) &&                
                Arguments.Count() == expression.Arguments.Count() &&
                Arguments.All((a, b) => a.IsSameExpression(expression.Arguments.ElementAt(b)));
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
    }
}
