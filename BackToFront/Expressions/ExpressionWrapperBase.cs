﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Utilities;

using BackToFront.Extensions.Reflection;
using BackToFront.Meta;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Expressions
{
    [System.Diagnostics.DebuggerDisplay("{WrappedExpression}")]
    public abstract class ExpressionWrapperBase
    {
        private readonly Dictionary<ParameterExpression, IEnumerable<MemberChainItem>> _MembersCache = new Dictionary<ParameterExpression, IEnumerable<MemberChainItem>>();
        public static readonly ReadOnlyDictionary<Type, Func<Expression, ExpressionWrapperBase>> Constructors;

        static ExpressionWrapperBase()
        {
            var constructors = new Dictionary<Type, Func<Expression, ExpressionWrapperBase>>();

            constructors[typeof(BinaryExpression)] = expression => new BinaryExpressionWrapper(expression as BinaryExpression);
            constructors[typeof(ConstantExpression)] = expression => new ConstantExpressionWrapper(expression as ConstantExpression);
            constructors[typeof(MethodCallExpression)] = expression => new MethodCallExpressionWrapper(expression as MethodCallExpression);
            constructors[typeof(UnaryExpression)] = expression => new UnaryExpressionWrapper(expression as UnaryExpression);
            constructors[typeof(ParameterExpression)] = expression => new ParameterExpressionWrapper(expression as ParameterExpression);
            constructors[typeof(MemberExpression)] = expression => new MemberExpressionWrapper(expression as MemberExpression);
            constructors[typeof(BlockExpression)] = expression => new BlockExpressionWrapper(expression as BlockExpression);
            constructors[typeof(ConditionalExpression)] = expression => new ConditionalExpressionWrapper(expression as ConditionalExpression);
            constructors[typeof(DefaultExpression)] = expression => new DefaultExpressionWrapper(expression as DefaultExpression);
            constructors[typeof(InvocationExpression)] = expression => new InvocationExpressionWrapper(expression as InvocationExpression);
            constructors[typeof(NewExpression)] = expression => new NewExpressionWrapper(expression as NewExpression);

            Constructors = new ReadOnlyDictionary<Type, Func<Expression, ExpressionWrapperBase>>(constructors);
        }

        public abstract Expression WrappedExpression { get; }

        /// <summary>
        /// Return all of the members in the Expression which have the input parameter as their root
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected abstract IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter);

        private IEnumerable<ParameterExpression> CachedUnorderedParameters;

        /// <summary>
        /// Gets all of the ParameterExpressions used in this expression
        /// </summary>
        public IEnumerable<ParameterExpression> UnorderedParameters
        {
            get
            {
                if (CachedUnorderedParameters == null)
                {
                    CachedUnorderedParameters = _UnorderedParameters;
                }

                return CachedUnorderedParameters;
            }
        }

        /// <summary>
        /// Return all of the Parameters used in this Expression and its inner expressions
        /// </summary>
        protected abstract IEnumerable<ParameterExpression> _UnorderedParameters { get; }

        public virtual bool IsSameExpression(Expression expression)
        {
            return expression != null && expression.NodeType == WrappedExpression.NodeType;
        }

        public static ExpressionWrapperBase CreateExpressionWrapper(Expression expression)
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

            return Constructors[type](expression);
        }

        /// <summary>
        /// Gets all of the members in the Expression which have the input parameter as their root
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<MemberChainItem> GetMembersForParameter(ParameterExpression parameter)
        {
            if (!_MembersCache.ContainsKey(parameter))
            {
                // only cache members from UnorderedParameters, caching everything could
                // conceivably create a giant cache in what is probably a static object
                if (!UnorderedParameters.Contains(parameter))
                {
                    return _GetMembersForParameter(parameter);
                }

                _MembersCache.Add(parameter, _GetMembersForParameter(parameter).ToArray());
            }
            
            return _MembersCache[parameter];
        }

        /// <summary>
        /// Is this expression contains the expression part "child", this will be replaced with the expression part "root"
        ///     Both the InnerExpression and input expressions must have one argument and it has to be the same type
        ///     The input expression must be fully linear (ILinearExpression)
        ///     There is no room for error. If there is no mock, an exception will be thrown
        ///     All instances of the original parameters of InnerExpression must be mocked out
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="child"></param>
        /// <param name="root">The root of the returned expression</param>
        /// <returns></returns>
        public Expression ForChildExpression<TEntity, TChild>(Expression<Func<TEntity, TChild>> child, Expression root)
        {
            if (!(this is ILinearExpression))
                throw new InvalidOperationException("##");

            if (UnorderedParameters.Count() != 1 ||
                !child.Parameters.First().Type.Is(UnorderedParameters.First().Type))
                throw new InvalidOperationException("##");

            var wrapper = ExpressionWrapperBase.ToWrapper(child);
            if (!(wrapper is ILinearExpression))
                throw new InvalidOperationException("##");

            // ensure only ILinearExpressions get into this stack
            Stack<ExpressionWrapperBase> linearExpressionBuilder = new Stack<ExpressionWrapperBase>();
            linearExpressionBuilder.Push(this);

            while (true)
            {
                var last = linearExpressionBuilder.Peek();
                if (last.IsSameExpression(wrapper.WrappedExpression))
                {
                    // remove last expression, this will be the expression we are mocking out
                    linearExpressionBuilder.Pop();
                    Expression current = root;
                    while (linearExpressionBuilder.Count > 0)
                    {
                        current = (linearExpressionBuilder.Pop() as ILinearExpression).WithAlternateRoot(current, child).WrappedExpression;
                    }

                    return current;
                }

                if (!(last is ILinearExpression))
                {
                    break;
                }
                else if (!((last as ILinearExpression).Root is ILinearExpression))
                {
                    break;
                }
                else
                {
                    linearExpressionBuilder.Push((last as ILinearExpression).Root);
                }
            }

            // there is a non-linear expression in there somewhere
            throw new InvalidOperationException("##");
        }

        #region linq constructors

        public static ExpressionWrapperBase ToWrapper<TEntity, TReturn>(Expression<Func<TEntity, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TReturn>(Expression<Func<TEntity, TArg2, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
        }

        public static ExpressionWrapperBase ToWrapper<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TReturn>(Expression<Func<TEntity, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TReturn>> expression)
        {
            return CreateExpressionWrapper(expression.Body);
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
