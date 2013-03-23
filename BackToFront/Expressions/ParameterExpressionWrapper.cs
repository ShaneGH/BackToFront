using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Extensions.Reflection;
using BackToFront.Utils;

namespace BackToFront.Expressions
{
    public class ParameterExpressionWrapper : ExpressionWrapperBase<ParameterExpression>, IPropertyChainGetter
    {
        public ParameterExpressionWrapper(ParameterExpression expression)
            : base(expression)
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

        protected override Expression OnCompile(IEnumerable<Mock> mocks)
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
