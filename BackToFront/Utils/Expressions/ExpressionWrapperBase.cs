﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils.Expressions
{
    /* Unit test TODO:
    System.Linq.Expressions.BinaryExpression a; -
    System.Linq.Expressions.BlockExpression a;
    System.Linq.Expressions.ConditionalExpression a; -
    System.Linq.Expressions.ConstantExpression a; -
    System.Linq.Expressions.DebugInfoExpression a;
    System.Linq.Expressions.DefaultExpression a;
    System.Linq.Expressions.DynamicExpression a;
    System.Linq.Expressions.GotoExpression a;
    System.Linq.Expressions.IndexExpression a;
    System.Linq.Expressions.InvocationExpression a;
    System.Linq.Expressions.LabelExpression a;
    System.Linq.Expressions.LambdaExpression a;
    System.Linq.Expressions.ListInitExpression a;
    System.Linq.Expressions.LoopExpression a;
    System.Linq.Expressions.MemberExpression a;
    System.Linq.Expressions.NewArrayExpression a;
    System.Linq.Expressions.NewExpression a; -
    System.Linq.Expressions.ParameterExpression a;
    System.Linq.Expressions.RuntimeVariablesExpression a;
    System.Linq.Expressions.SwitchExpression a;
    System.Linq.Expressions.TryExpression a;
    System.Linq.Expressions.TypeBinaryExpression a;
    System.Linq.Expressions.UnaryExpression a;
     */

    internal abstract class ExpressionWrapperBase
    {
        public abstract bool IsSameExpression(ExpressionWrapperBase expression);
        public abstract ReadOnlyCollection<ParameterExpression> WrappedExpressionParameters { get; }

        /// <summary>
        /// Evaluate expression assuming correct parameters have been passed in
        /// </summary>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        protected abstract object OnEvaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks);

        public object Evaluate(IEnumerable<object> paramaters)
        {
            return Evaluate(paramaters, Enumerable.Empty<Tuple<ExpressionWrapperBase, object>>());
        }

        public object Evaluate<TEntity, TOutput>(IEnumerable<object> paramaters, IEnumerable<Tuple<Expression<Func<TEntity, TOutput>>, TOutput>> mocks)
        {
            return Evaluate(paramaters, mocks.Select(m => new Tuple<ExpressionWrapperBase, object>(new FuncExpressionWrapper<TEntity, TOutput>(m.Item1).Body, m.Item2)).ToArray());
        }

        public object Evaluate(IEnumerable<object> paramaters, IEnumerable<Tuple<ExpressionWrapperBase, object>> mocks)
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
                if (IsSameExpression(mock.Item1))
                    return mock.Item2;

            return OnEvaluate(param, mocks);
        }
    }

    internal abstract class ExpressionWrapperBase<TExpression> : ExpressionWrapperBase
        where TExpression : Expression
    {
        protected readonly TExpression Expression;

        private ReadOnlyCollection<ParameterExpression> _Parameters;
        protected ReadOnlyCollection<ParameterExpression> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                if (Expression is LambdaExpression)
                    throw new InvalidOperationException("Cannot set the parameter type if the expression has parameters already");

                _Parameters = new ReadOnlyCollection<ParameterExpression>(value);
            }
        }

        public override ReadOnlyCollection<ParameterExpression> WrappedExpressionParameters
        {
            get { return Parameters; }
        }

        public ExpressionWrapperBase(TExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Expression = expression;
            if (Expression is LambdaExpression)
                _Parameters = (Expression as LambdaExpression).Parameters;
        }

        protected ExpressionWrapperBase CreateChildWrapper(Expression expression)
        {
            if (expression is BinaryExpression)
                return new BinaryExpressionWrapper(expression as BinaryExpression) { Parameters = this.Parameters };
            else if (expression is BlockExpression)
                throw new NotImplementedException();
            else if (expression is ConditionalExpression)
                throw new NotImplementedException();
            else if (expression is ConstantExpression)
                return new ConstantExpressionWrapper(expression as ConstantExpression) { Parameters = this.Parameters };
            else if (expression is DebugInfoExpression)
                throw new NotImplementedException();
            else if (expression is DefaultExpression)
                throw new NotImplementedException();
            else if (expression is DynamicExpression)
                throw new NotImplementedException();
            else if (expression is GotoExpression)
                throw new NotImplementedException();
            else if (expression is IndexExpression)
                throw new NotImplementedException();
            else if (expression is InvocationExpression)
                throw new NotImplementedException();
            else if (expression is LabelExpression)
                throw new NotImplementedException();
            else if (expression is ListInitExpression)
                throw new NotImplementedException();
            else if (expression is LoopExpression)
                throw new NotImplementedException();
            else if (expression is MemberExpression)
                return new MemberExpressionWrapper(expression as MemberExpression) { Parameters = this.Parameters };
            else if (expression is MethodCallExpression)
                return new MethodCallExpressionWrapper(expression as MethodCallExpression) { Parameters = this.Parameters };
            else if (expression is NewArrayExpression)
                throw new NotImplementedException();
            else if (expression is NewExpression)
                throw new NotImplementedException();
            else if (expression is ParameterExpression)
                return new ParameterExpressionWrapper(expression as ParameterExpression) { Parameters = this.Parameters };
            else if (expression is RuntimeVariablesExpression)
                throw new NotImplementedException();
            else if (expression is SwitchExpression)
                throw new NotImplementedException();
            else if (expression is TryExpression)
                throw new NotImplementedException();
            else if (expression is TypeBinaryExpression)
                throw new NotImplementedException();
            else if (expression is UnaryExpression)
                return new UnaryExpressionWrapper(expression as UnaryExpression) { Parameters = this.Parameters };

            // LambdaExpression is not implemented
            throw new NotImplementedException();
        }
    }
}