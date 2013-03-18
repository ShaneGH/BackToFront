using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utils.Expressions
{
    /// <summary>
    /// Attempts to wrap a call to a constant linq valiable (from a linq expression of defined in an outer context). 
    /// In reality the linq constant is a property on a compiler generated class with 1 field: the linq parameter's name
    /// </summary>
    public class LinqParameterExpressionWrapper : MemberExpressionWrapper
    {
        // the field name of the property in the compiler generated class
        public readonly string CompilerGenerated;

        public LinqParameterExpressionWrapper(MemberExpression expression)
            : base(expression)
        {
            CompilerGenerated = GetCompilerGeneratedProperty(expression);
        }

        public static bool IsLinqParameter(MemberExpression exp)
        {
            return !string.IsNullOrEmpty(GetCompilerGeneratedProperty(exp));
        }

        private static string GetCompilerGeneratedProperty(MemberExpression expression)
        {
            if (expression.Expression is ConstantExpression && expression.Expression.Type.IsDefined(typeof(CompilerGeneratedAttribute), true))
            {
                var exp = expression.Expression as ConstantExpression;
                var members = exp.Value.GetType().GetFields();
                if (members.Count() == 1)
                {
                    return (members.First() as FieldInfo).Name;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (!string.IsNullOrEmpty(CompilerGenerated) && 
                expression.WrappedExpression is ConstantExpression && 
                (expression.WrappedExpression as ConstantExpression).Value is BackToFront.Logic.Dependency)
            {
                var val = (expression.WrappedExpression as ConstantExpression).Value as BackToFront.Logic.Dependency;
                if (val.Name == CompilerGenerated)
                    return true;
                else
                    return false;
            }

            return base.IsSameExpression(expression);
        }
    }
}
