using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using E = System.Linq.Expressions;

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
            var ex = expression as ParameterExpressionWrapper;
            if (ex == null)
                return false;

            return Index >= 0 && Index == ex.Index;
        }

        private int Index
        {
            get
            {
                return Parameters.IndexOf(Expression);
            }
        }

        protected override Expression OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks)
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
