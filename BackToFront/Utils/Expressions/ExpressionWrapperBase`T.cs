using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils.Expressions
{
    internal abstract class ExpressionWrapperBase<TExpression> : ExpressionWrapperBase
        where TExpression : Expression
    {
        public readonly TExpression Expression;

        protected readonly ReadOnlyCollection<ParameterExpression> Parameters;

        public override ReadOnlyCollection<ParameterExpression> WrappedExpressionParameters
        {
            get { return Parameters; }
        }

        public ExpressionWrapperBase(TExpression expression, ReadOnlyCollection<ParameterExpression> parameters)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if (parameters.Count == 0)
                throw new ArgumentException("The expression must contain paramaters");

            Expression = expression;
            Parameters = parameters;
        }

        protected ExpressionWrapperBase CreateChildWrapper(Expression expression)
        {
            return CreateChildWrapper(expression, Parameters);
        }
    }
}
