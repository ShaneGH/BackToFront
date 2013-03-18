using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using E = System.Linq.Expressions;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils.Expressions
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
            var ex = expression as MemberExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Member == Expression.Member && InnerExpression.IsSameExpression(ex.InnerExpression);
        }

        // TODO, what if member is event or other memberinfo
        protected override Expression OnEvaluate(IEnumerable<Mock> mocks)
        {
            var eval = InnerExpression.Evaluate(mocks);
            Expression returnVal;
            if (eval == InnerExpression.WrappedExpression)
                returnVal = Expression;
            else
                returnVal= E.Expression.MakeMemberAccess(eval, Expression.Member);

            return returnVal;
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
                return (InnerExpression as IPropertyChainGetter).Get(root);
            }
            else
            {
                throw new InvalidOperationException("##");
            }
        }
    }
}
