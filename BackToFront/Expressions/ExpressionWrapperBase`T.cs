using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;
using BackToFront.Meta;

namespace BackToFront.Expressions
{
    public abstract class ExpressionWrapperBase<TExpression> : ExpressionWrapperBase
        where TExpression : Expression
    {
        public readonly TExpression Expression;

        public ExpressionWrapperBase(TExpression expression)
        {
            Expression = expression;
        }

        public override Expression WrappedExpression
        {
            get 
            {
                return Expression;
            }
        }

        protected abstract bool _IsSameExpression(TExpression expression);

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            if (expression is TExpression)
                return _IsSameExpression(expression as TExpression);

            return false;
        }

        protected static ExpressionWrapperBase CreateOrReference(Expression expression, ref ExpressionWrapperBase wrapper)
        {
            return wrapper ?? (wrapper = ExpressionWrapperBase.CreateChildWrapper(expression));
        }

        protected static ExpressionWrapperBase[] CreateOrReference(IEnumerable<Expression> expression, ref ExpressionWrapperBase[] wrapper)
        {
            return wrapper ?? (wrapper = expression.Select(a => ExpressionWrapperBase.CreateChildWrapper(a)).ToArray());
        }
    }
}
