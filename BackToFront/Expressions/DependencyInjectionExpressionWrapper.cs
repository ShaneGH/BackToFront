using BackToFront.Dependency;
using BackToFront.Logic;
using BackToFront.Utilities;
using System;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Meta;
using System.Collections.Generic;
using BackToFront.Enum;
using System.Runtime.Serialization;

namespace BackToFront.Expressions
{
    /// <summary>
    /// Attempts to wrap a call to The Val Property of Dependency<> and mock it our for its actual dependency
    /// If the perscribed property is not Val, defaults to regular MemberExpressionWrapper behavior
    /// </summary>
    public sealed class DependencyInjectionExpressionWrapper : MemberExpressionWrapper
    {
        // the field name of the property in the compiler generated class
        public readonly string LinqParamName;

        private DependencyInjectionExpressionWrapper(MemberExpression expression, string compilerGenerated)
            : base(expression)
        {
            if (string.IsNullOrEmpty(compilerGenerated))
                throw new ArgumentNullException("compilerGenerated");

            LinqParamName = compilerGenerated;
        }

        public DependencyInjectionExpressionWrapper(MemberExpression expression)
            : this(expression, GetLinqParamName(expression))
        {
        }

        public static bool TryCreate(MemberExpression expression, out DependencyInjectionExpressionWrapper result)
        {
            var compilerGenerated = GetLinqParamName(expression);
            if (string.IsNullOrEmpty(compilerGenerated))
            {
                result = null;
                return false;
            }
            else
            {
                result = new DependencyInjectionExpressionWrapper(expression, compilerGenerated);
                return true;
            }
        }

        private static string GetLinqParamName(MemberExpression expression)
        {
            if (expression.Member == DependencyWrapper.ValProperty(expression.Type))
            {
                // Dependency<T> is only constructed in one piece of code, so we are assuming that this expression is safe
                return E.Expression.Lambda<Func<DependencyWrapper>>(expression.Expression).Compile()().DependencyName;

                //TODO: 2 possible exceptions
                //return E.Expression.Lambda<Func<BackToFront.Framework.XXX>>(E.Expression.Parameter(typeof(string))).Compile()().Name;
                //return E.Expression.Lambda<Func<BackToFront.Framework.XXX>>(E.Expression.Parameter(typeof(BackToFront.Framework.XXX))).Compile()().Name;
            }
            else
            {
                return null;
            }
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            // do not use base methods, they are not valid in this special case

            if (expression.WrappedExpression is ConstantExpression && 
                (expression.WrappedExpression as ConstantExpression).Value is RuleDependency)
            {
                return ((expression.WrappedExpression as ConstantExpression).Value as RuleDependency).Name == LinqParamName;
            }

            return base.IsSameExpression(expression);
        }

        private ExpressionElementMeta _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ExpressionElementMeta(null, new ExpressionElementMeta[0], ExpressionWrapperType.DependencyInjection, Expression.Type, null));
            }
        }
    }
}
