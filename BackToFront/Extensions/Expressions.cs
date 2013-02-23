using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BackToFront.Extensions.Expressions
{
    public static class Expressions
    {
        public static IEnumerable<MemberInfo> AsProperty<T>(this Expression<Func<T, object>> expression)
        {
            return ReadExpression(expression.Body);
        }

        static IEnumerable<MemberInfo> ReadExpression(Expression expression)
        {
            List<MemberInfo> output = new List<MemberInfo>();

            while (expression != null)
            {
                if (expression is UnaryExpression)
                    output.AddRange(NextExpression(expression as UnaryExpression, out expression));
                else if (expression is MemberExpression)
                    output.AddRange(NextExpression(expression as MemberExpression, out expression));
                else if (expression is MethodCallExpression)
                    output.AddRange(NextExpression(expression as MethodCallExpression, out expression));
                else if (expression is BinaryExpression)
                    output.AddRange(NextExpression(expression as BinaryExpression, out expression));
                else if (expression is ConditionalExpression)
                    output.AddRange(NextExpression(expression as ConditionalExpression, out expression));
                else if (expression is ConstantExpression)
                    return Enumerable.Empty<MemberInfo>();
                else
                    break;
            }

            return output.Where(a => a != null).Distinct();
        }

        static IEnumerable<MemberInfo> NextExpression(UnaryExpression ex, out Expression next)
        {
            next = ex.Operand;
            return new[] { ex.Method };
        }

        static IEnumerable<MemberInfo> NextExpression(MemberExpression ex, out Expression next)
        {
            next = ex.Expression;
            return new[] { ex.Member };
        }

        static IEnumerable<MemberInfo> NextExpression(MethodCallExpression ex, out Expression next)
        {
            List<MemberInfo> info = new List<MemberInfo>();
            next = ex.Object;
            info.Add(ex.Method);
            if (ex.Arguments != null)
            {
                foreach (var arg in ex.Arguments)
                    info.AddRange(ReadExpression(arg));
            }

            return info.ToArray();
        }

        static IEnumerable<MemberInfo> NextExpression(BinaryExpression ex, out Expression next)
        {
            next = null;
            List<MemberInfo> output = new List<MemberInfo>();
            output.AddRange(ReadExpression(ex.Left));
            output.AddRange(ReadExpression(ex.Right));

            return output.Where(a => a != null).Distinct();
        }

        static IEnumerable<MemberInfo> NextExpression(ConditionalExpression ex, out Expression next)
        {
            next = null;
            List<MemberInfo> output = new List<MemberInfo>();
            output.AddRange(ReadExpression(ex.Test));
            output.AddRange(ReadExpression(ex.IfFalse));
            output.AddRange(ReadExpression(ex.IfTrue));

            return output.Where(a => a != null).Distinct();
        }
    }
}
