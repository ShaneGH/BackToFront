using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils.Expressions
{
    public class ParameterExpressionWrapper : ExpressionWrapperBase<ParameterExpression>, IPropertyChainGetter
    {
        public ParameterExpressionWrapper(ParameterExpression expression, ReadOnlyCollection<ParameterExpression> paramaters)
            : base(expression, paramaters)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            // TODO: is this correct?

            var ex = expression as ParameterExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Type.Is(Expression.Type);
        }

        protected override Expression OnEvaluate(IEnumerable<Mock> mocks)
        {
            return Expression;
        }

        public object Get(object root)
        {
            if (Expression.Type != root.GetType())
            {
                throw new InvalidOperationException("##");
            }

            return root;
        }
    }
}
