using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Utils;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Expressions
{
    public abstract class ExpressionWrapperBase
    {
        public static readonly ReadOnlyDictionary<Type, Func<Expression, ExpressionWrapperBase>> Constructors;
        private static readonly Dictionary<Type, Func<Expression, ExpressionWrapperBase>> _Constructors = new Dictionary<Type, Func<Expression, ExpressionWrapperBase>>();

        public static readonly ReadonlyDictionary<ExpressionType, Func<Expression, Expression, Expression>> Evaluations;
        private static readonly Dictionary<ExpressionType, Func<Expression, Expression, Expression>> _Evaluations = new Dictionary<ExpressionType, Func<Expression, Expression, Expression>>();

        static ExpressionWrapperBase()
        {
            Evaluations = new ReadonlyDictionary<ExpressionType, Func<Expression, Expression, Expression>>(_Evaluations);
            Constructors = new ReadOnlyDictionary<Type, Func<Expression, ExpressionWrapperBase>>(_Constructors);
            
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
            _Evaluations[ExpressionType.Power] = (lhs, rhs) => Expression.Power(lhs, rhs);
            _Evaluations[ExpressionType.Subtract] = (lhs, rhs) => Expression.Subtract(lhs, rhs);

            _Constructors[typeof(BinaryExpression)] = expression => new BinaryExpressionWrapper(expression as BinaryExpression);
            _Constructors[typeof(ConstantExpression)] = expression => new ConstantExpressionWrapper(expression as ConstantExpression);
            _Constructors[typeof(MemberExpression)] = expression =>
            {
                DependencyInjectionExpressionWrapper result;
                return DependencyInjectionExpressionWrapper.TryCreate(expression as MemberExpression, out result) ? result :
                    (ExpressionWrapperBase)new MemberExpressionWrapper(expression as MemberExpression);
            };
            _Constructors[typeof(MethodCallExpression)] = expression => new MethodCallExpressionWrapper(expression as MethodCallExpression);
            _Constructors[typeof(UnaryExpression)] = expression => new UnaryExpressionWrapper(expression as UnaryExpression);
            _Constructors[typeof(ParameterExpression)] = expression => new ParameterExpressionWrapper(expression as ParameterExpression);
        }

        /// <summary>
        /// Evaluate expression assuming correct parameters have been passed in
        /// </summary>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        protected abstract Expression OnCompile(IEnumerable<Mock> mocks);
        public abstract bool IsSameExpression(ExpressionWrapperBase expression);
        public abstract Expression WrappedExpression { get; }
                
        public Expression Compile()
        {
            return WrappedExpression;
        }

        public Expression Compile(IEnumerable<Mock> mocks)
        {
            if (mocks == null || mocks.Count() == 0)
                return Compile();

            foreach (var mock in mocks)
                if (IsSameExpression(mock.Expression))
                        return mock.Value;

            return OnCompile(mocks);
        }

        public static ExpressionWrapperBase CreateChildWrapper(Expression expression)
        {
            var type = expression.GetType();

            while (type != typeof(Expression))
            {
                if (Constructors.ContainsKey(type))
                    break;
                else
                    type = type.BaseType;
            }

            if (type == typeof(Expression))
                throw new InvalidOperationException("##" + expression.GetType().ToString());
            // TODO: Key not found exception
            return Constructors[type](expression);
        }

        #region linq constructors

        public static ExpressionWrapperBase ToWrapper<TEntity, TReturn>(Expression<Func<TEntity, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TReturn>(Expression<Func<TEntity, TArg2, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TReturn>> expression)
        {
            return CreateChildWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TReturn>(Expression<Func<TEntity, TReturn>> expression, out ReadOnlyCollection<ParameterExpression> parameters)
        {
            parameters = expression.Parameters;
            return ToWrapper(expression);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TReturn>(Expression<Func<TEntity, TArg2, TReturn>> expression, out ReadOnlyCollection<ParameterExpression> parameters)
        {
            parameters = expression.Parameters;
            return ToWrapper(expression);
        }

        #endregion
    }
}
