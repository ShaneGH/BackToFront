using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

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

        // TODO: Make protected (this is a big job for unit testing)
        public abstract bool IsSameExpression(TExpression expression);

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            if (expression is TExpression)
                return IsSameExpression(expression as TExpression);

            return false;
        }
    }
}
