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
    internal abstract class ExpressionWrapperBase
    {
        public static readonly ReadOnlyDictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>> Constructors;
        private static readonly Dictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>> _Constructors = new Dictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>>();
        
        internal static readonly ReadonlyDictionary<ExpressionType, Func<dynamic, dynamic, dynamic>> Evaluations;
        private static readonly Dictionary<ExpressionType, Func<dynamic, dynamic, dynamic>> _Evaluations = new Dictionary<ExpressionType, Func<dynamic, dynamic, dynamic>>();

        static ExpressionWrapperBase()
        {
            Evaluations = new ReadonlyDictionary<ExpressionType, Func<dynamic, dynamic, dynamic>>(_Evaluations);
            Constructors = new ReadOnlyDictionary<Type, Func<Expression, ReadOnlyCollection<ParameterExpression>, ExpressionWrapperBase>>(_Constructors);
            
            // add operators
            _Evaluations[ExpressionType.Add] = (lhs, rhs) => lhs + rhs;
            _Evaluations[ExpressionType.AndAlso] = (lhs, rhs) => lhs && rhs;
            _Evaluations[ExpressionType.ArrayIndex] = (lhs, rhs) => lhs[rhs];
            _Evaluations[ExpressionType.Coalesce] = (lhs, rhs) => lhs ?? rhs;
            _Evaluations[ExpressionType.Convert] = (lhs, rhs) => Convert.ChangeType(lhs, rhs);
            _Evaluations[ExpressionType.Divide] = (lhs, rhs) => lhs / rhs;
            _Evaluations[ExpressionType.Equal] = (lhs, rhs) => lhs == rhs;
            _Evaluations[ExpressionType.ExclusiveOr] = (lhs, rhs) => lhs ^ rhs;
            _Evaluations[ExpressionType.GreaterThan] = (lhs, rhs) => lhs > rhs;
            _Evaluations[ExpressionType.GreaterThanOrEqual] = (lhs, rhs) => lhs >= rhs;
            _Evaluations[ExpressionType.LessThan] = (lhs, rhs) => lhs < rhs;
            _Evaluations[ExpressionType.LessThanOrEqual] = (lhs, rhs) => lhs <= rhs;
            _Evaluations[ExpressionType.Modulo] = (lhs, rhs) => lhs % rhs;
            _Evaluations[ExpressionType.Multiply] = (lhs, rhs) => lhs * rhs;
            _Evaluations[ExpressionType.Not] = (lhs, rhs) => _isIntegralType(lhs) ? ~lhs : !lhs;
            _Evaluations[ExpressionType.NotEqual] = (lhs, rhs) => lhs != rhs;
            _Evaluations[ExpressionType.OrElse] = (lhs, rhs) => lhs || rhs;
            // untested (visual basic operator)
            _Evaluations[ExpressionType.Power] = (lhs, rhs) => Math.Pow(lhs, rhs);
            _Evaluations[ExpressionType.Subtract] = (lhs, rhs) => lhs - rhs;

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
        protected abstract object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks);
        public abstract bool IsSameExpression(ExpressionWrapperBase expression);
        public abstract ReadOnlyCollection<ParameterExpression> WrappedExpressionParameters { get; }
                
        public object Evaluate(IEnumerable<object> paramaters)
        {
            return Evaluate(paramaters, Enumerable.Empty<Mock>());
        }

        public object Evaluate(IEnumerable<object> paramaters, IEnumerable<Mock> mocks)
        {
            var param = paramaters.ToArray();
            if (param.Length > WrappedExpressionParameters.Count)
                throw new InvalidOperationException("##");

            for (int i = 0, ii = param.Length; i < ii; i++)
            {
                if ((WrappedExpressionParameters[i].Type.IsValueType && param[i] == null) ||
                    (param[i] != null && !param[i].GetType().Is(WrappedExpressionParameters[i].Type)))
                    throw new InvalidOperationException("##"); // invalid parameter type
            }

            foreach (var mock in mocks)
                if (IsSameExpression(mock.Expression))
                    return mock.Value;

            return OnEvaluate(param, mocks);
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
