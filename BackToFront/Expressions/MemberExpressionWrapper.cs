using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Utils;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;

namespace BackToFront.Expressions
{
    public class MemberExpressionWrapper : ExpressionWrapperBase<MemberExpression>, IPropertyChain
    {
        private ExpressionWrapperBase _InnerExpression;
        private ExpressionWrapperBase InnerExpression
        {
            get            
            {
                return _InnerExpression ?? (_InnerExpression = CreateChildWrapper(Expression.Expression));
            }
        }

        public MemberExpressionWrapper(MemberExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as MemberExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Member == Expression.Member && InnerExpression.IsSameExpression(ex.InnerExpression);
        }

        // TODO, what if member is event or other memberinfo
        protected override Expression CompileInnerExpression(Mocks mocks)
        {
            var eval = InnerExpression.Compile(mocks);
            return eval == InnerExpression.WrappedExpression ? 
                Expression :
                E.Expression.MakeMemberAccess(eval, Expression.Member);
        }

        public bool CanSet
        {
            get 
            {
                return InnerExpression is ParameterExpressionWrapper ||
                    (InnerExpression is IPropertyChain && (InnerExpression as IPropertyChain).CanSet); 
            }
        }

        public void Set(object root, object value)
        {
            if (InnerExpression is IPropertyChainGetter)
            {
                root = (InnerExpression as IPropertyChainGetter).Get(root);
            }
            else
            {
                throw new InvalidOperationException("##");
            }

            Expression.Member.Set(root, value);
        }

        public object Get(object root)
        {
            if (InnerExpression is IPropertyChainGetter)
            {
                root = (InnerExpression as IPropertyChainGetter).Get(root);
                return Expression.Member.Get(root);
            }
            else
            {
                throw new InvalidOperationException("##");
            }
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            var root = InnerExpression.GetMembersForParameter(parameter);
            root.Each(r => r.SetNext(Expression.Member));
            return root;
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return InnerExpression.UnorderedParameters;
            }
        }
    }
}
