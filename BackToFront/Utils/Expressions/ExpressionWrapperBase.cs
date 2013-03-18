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
    public abstract class ExpressionWrapperBase
    {
        public static readonly ReadOnlyDictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>> Constructors;
        private static readonly Dictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>> _Constructors = new Dictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>>();

        public static readonly ReadonlyDictionary<ExpressionType, Func<Expression, Expression, Expression>> Evaluations;
        private static readonly Dictionary<ExpressionType, Func<Expression, Expression, Expression>> _Evaluations = new Dictionary<ExpressionType, Func<Expression, Expression, Expression>>();

        static ExpressionWrapperBase()
        {
            Evaluations = new ReadonlyDictionary<ExpressionType, Func<Expression, Expression, Expression>>(_Evaluations);
            Constructors = new ReadOnlyDictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>>(_Constructors);
            
            //TODO: All of this needs to be changed. Static constructor methods cannot be bundled together like this

            // add operators
            _Evaluations[ExpressionType.Add] = (lhs, rhs) => Expression.Add(lhs, rhs);
            _Evaluations[ExpressionType.AndAlso] = (lhs, rhs) => Expression.AndAlso(lhs, rhs);
            _Evaluations[ExpressionType.ArrayIndex] = (lhs, rhs) => Expression.ArrayIndex(lhs, rhs);
            _Evaluations[ExpressionType.Coalesce] = (lhs, rhs) => Expression.Coalesce(lhs, rhs);
            //TODO: this is wrong
            _Evaluations[ExpressionType.Convert] = (lhs, rhs) => Expression.Convert(lhs, rhs.Type);
            _Evaluations[ExpressionType.Divide] = (lhs, rhs) => Expression.Divide(lhs, rhs);
            _Evaluations[ExpressionType.Equal] = (lhs, rhs) => Expression.Equal(lhs, rhs);
            _Evaluations[ExpressionType.ExclusiveOr] = (lhs, rhs) => Expression.ExclusiveOr(lhs, rhs);
            _Evaluations[ExpressionType.GreaterThan] = (lhs, rhs) => Expression.GreaterThan(lhs, rhs);
            _Evaluations[ExpressionType.GreaterThanOrEqual] = (lhs, rhs) => Expression.GreaterThanOrEqual(lhs, rhs);
            _Evaluations[ExpressionType.LessThan] = (lhs, rhs) => Expression.LessThan(lhs, rhs);
            _Evaluations[ExpressionType.LessThanOrEqual] = (lhs, rhs) => Expression.LessThanOrEqual(lhs, rhs);
            _Evaluations[ExpressionType.Modulo] = (lhs, rhs) => Expression.Modulo(lhs, rhs);
            _Evaluations[ExpressionType.Multiply] = (lhs, rhs) => Expression.Multiply(lhs, rhs);
            // todo unaryExpression method
            _Evaluations[ExpressionType.Not] = (lhs, rhs) => Expression.Not(lhs);
            _Evaluations[ExpressionType.NotEqual] = (lhs, rhs) => Expression.NotEqual(lhs, rhs);
            _Evaluations[ExpressionType.OrElse] = (lhs, rhs) => Expression.OrElse(lhs, rhs);
            // untested (visual basic operator)
            _Evaluations[ExpressionType.Power] = (lhs, rhs) => Expression.Power(lhs, rhs);
            _Evaluations[ExpressionType.Subtract] = (lhs, rhs) => Expression.Subtract(lhs, rhs);

            _Constructors[typeof(BinaryExpression)] = (expression, prameters) => new BinaryExpressionWrapper(expression as BinaryExpression, prameters);
            _Constructors[typeof(ConstantExpression)] = (expression, parameters) => new ConstantExpressionWrapper(expression as ConstantExpression, parameters);
            _Constructors[typeof(MemberExpression)] = (expression, parameters) => new MemberExpressionWrapper(expression as MemberExpression, parameters);
            _Constructors[typeof(MethodCallExpression)] = (expression, parameters) => new MethodCallExpressionWrapper(expression as MethodCallExpression, parameters);
            _Constructors[typeof(UnaryExpression)] = (expression, parameters) => new UnaryExpressionWrapper(expression as UnaryExpression, parameters);
            _Constructors[typeof(ParameterExpression)] = (expression, parameters) => new ParameterExpressionWrapper(expression as ParameterExpression, parameters);
        }

        private static bool _isIntegralType(dynamic item)
        {
            return item is sbyte || item is byte || item is char || item is short ||
                item is ushort || item is int || item is uint || item is long ||
                item is ulong;
        }

        /// <summary>
        /// Evaluate expression assuming correct parameters have been passed in
        /// </summary>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        protected abstract Expression OnEvaluate(IEnumerable<Mock> mocks);
        public abstract bool IsSameExpression(ExpressionWrapperBase expression);
        public abstract ReadOnlyCollection<ParameterExpression> WrappedExpressionParameters { get; }
        public abstract Expression WrappedExpression { get; }
                
        public Expression Evaluate()
        {
            // TODO: just return expression
            return Evaluate(Enumerable.Empty<Mock>());
        }

        public Expression Evaluate(IEnumerable<Mock> mocks)
        {
            foreach (var mock in mocks)
                if (IsSameExpression(mock.Expression))
                    return mock.Value;

            return OnEvaluate(mocks);
        }

        public static ExpressionWrapperBase CreateChildWrapper(Expression expression, ReadOnlyCollection<ParameterExpression> paramaters)
        {
            var type = expression.GetType();

            while (type != typeof(Expression))
            {
                if (Constructors.ContainsKey(type))
                    break;
                else
                    type = type.BaseType;
            }

            // TODO: Key not found exception
            return Constructors[type](expression, paramaters);
        }

        #region linq constructors

        public static ExpressionWrapperBase ToWrapper<TEntity, TReturn>(Expression<Func<TEntity, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TReturn>(Expression<Func<TEntity, TArg2, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body, expression.Parameters);
        }

        #endregion
    }
}
