using System;
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

namespace BackToFront.Expressions
{
    [System.Diagnostics.DebuggerDisplay("{WrappedExpression}")]
    public abstract class ExpressionWrapperBase
    {
        private readonly Dictionary<ParameterExpression, IEnumerable<MemberChainItem>> _MembersCache = new Dictionary<ParameterExpression, IEnumerable<MemberChainItem>>();
        public static readonly ReadOnlyDictionary<Type, Func<Expression, ExpressionWrapperBase>> Constructors;
        private static readonly Dictionary<Type, Func<Expression, ExpressionWrapperBase>> _Constructors = new Dictionary<Type, Func<Expression, ExpressionWrapperBase>>();

        static ExpressionWrapperBase()
        {
            Constructors = new ReadOnlyDictionary<Type, Func<Expression, ExpressionWrapperBase>>(_Constructors);

            _Constructors[typeof(BinaryExpression)] = expression => new BinaryExpressionWrapper(expression as BinaryExpression);
            _Constructors[typeof(ConstantExpression)] = expression => new ConstantExpressionWrapper(expression as ConstantExpression);
            _Constructors[typeof(MethodCallExpression)] = expression => new MethodCallExpressionWrapper(expression as MethodCallExpression);
            _Constructors[typeof(UnaryExpression)] = expression => new UnaryExpressionWrapper(expression as UnaryExpression);
            _Constructors[typeof(ParameterExpression)] = expression => new ParameterExpressionWrapper(expression as ParameterExpression);
            _Constructors[typeof(MemberExpression)] = expression =>
            {
                DependencyInjectionExpressionWrapper result;
                return DependencyInjectionExpressionWrapper.TryCreate(expression as MemberExpression, out result) ? result :
                    (ExpressionWrapperBase)new MemberExpressionWrapper(expression as MemberExpression);
            };
        }

        /// <summary>
        /// Evaluate expression assuming correct parameters have been passed in
        /// </summary>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        protected abstract Expression CompileInnerExpression(Mocks mocks);
        public abstract Expression WrappedExpression { get; }
        protected abstract IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter);

        public abstract ExpressionElementMeta Meta { get; }

        private IEnumerable<ParameterExpression> CachedUnorderedParameters;
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

        protected abstract IEnumerable<ParameterExpression> _UnorderedParameters { get; }

        protected Expression CompileInnerExpression(IEnumerable<Mock> mocks)
        {
            return CompileInnerExpression(new Mocks(mocks));
        }

        public virtual bool IsSameExpression(Expression expression)
        {
            return expression != null && expression.NodeType == WrappedExpression.NodeType;
        }
                
        public Expression Compile()
        {
            return WrappedExpression;
        }

        public Expression Compile(params Mock[] mocks)
        {
            return Compile(new Mocks(mocks));
        }

        public Expression Compile(Mocks mocks)
        {
            if (mocks == null || mocks.Count() == 0)
                return Compile();

            var val = new Visitors.MockVisitor(mocks).Visit(WrappedExpression);
            return val != WrappedExpression ? val : CompileInnerExpression(mocks);
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

            return Constructors[type](expression);
        }

        public IEnumerable<MemberChainItem> GetMembersForParameter(ParameterExpression parameter)
        {
            if (!_MembersCache.ContainsKey(parameter))
            {
                _MembersCache.Add(parameter, _GetMembersForParameter(parameter).ToArray());
            }

            return _MembersCache[parameter];
        }

        /// <summary>
        /// Mocks part of the current expression with the given parameter with a specific set of rules:
        ///     Both the InnerExpression and input expressions must have one argument ahd it has the same type
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
            Stack<ExpressionWrapperBase> previous = new Stack<ExpressionWrapperBase>();
            previous.Push(this);

            while (true)
            {
                var last = previous.Peek();
                if (last.IsSameExpression(wrapper.WrappedExpression))
                {
                    // rempve last expression, this will be the expression we are mocking out
                    previous.Pop();
                    Expression current = root;
                    while (previous.Count > 0)
                    {
                        var next = previous.Pop();
                        current = (next as ILinearExpression).WithAlternateRoot(current, child).WrappedExpression;
                    }

                    return current;
                }

                if (!(last is ILinearExpression))
                {
                    break;
                }
                else
                {
                    if (!((last as ILinearExpression).Root is ILinearExpression))
                        break;

                    previous.Push((last as ILinearExpression).Root);
                }
            }

            // there is a non-linear expression in there somewhere
            throw new InvalidOperationException("##");
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
