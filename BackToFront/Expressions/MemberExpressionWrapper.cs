using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Utilities;
using E = System.Linq.Expressions;

namespace BackToFront.Expressions
{
    public class MemberExpressionWrapper : ExpressionWrapperBase<MemberExpression>, IPropertyChain
    {
        private ExpressionWrapperBase _InnerExpression;
        public ExpressionWrapperBase InnerExpression
        {
            get            
            {
                return _InnerExpression ?? (_InnerExpression =
                    (Expression.Expression == null ? new DefaultExpressionWrapper() : CreateExpressionWrapper(Expression.Expression)));
            }
        }

        public MemberExpressionWrapper(MemberExpression expression)
            : base(expression)
        {
        }

        protected override bool _IsSameExpression(MemberExpression expression)
        {
            return expression.Member == Expression.Member && InnerExpression.IsSameExpression(expression.Expression);
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

        public ExpressionWrapperBase Root
        {
            get { return InnerExpression; }
        }

        public ExpressionWrapperBase WithAlternateRoot<TEntity, TChild>(Expression root, Expression<Func<TEntity, TChild>> child)
        {
            return new MemberExpressionWrapper(E.Expression.PropertyOrField(root, Expression.Member.Name));
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            var root = InnerExpression.GetMembersForParameter(parameter);
            root.Each(r => r.NextItem = new MemberChainItem(Expression.Member));
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
