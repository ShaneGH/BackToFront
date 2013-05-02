using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Utilities;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

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

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as MemberExpression;
            if (ex == null)
                return false;

            return ex.Member == Expression.Member && InnerExpression.IsSameExpression(ex.Expression);
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

        private ExpressionElementMeta _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ExpressionElementMeta(Expression.Member.Name, new ExpressionElementMeta[0], ExpressionWrapperType.Member, Expression.Type, InnerExpression.Meta));
            }
        }
    }
}
