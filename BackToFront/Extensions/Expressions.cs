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
            List<MemberInfo> completedThreads = new List<MemberInfo>();
            List<MemberInfo> currentThread = new List<MemberInfo>();

            while (expression != null)
            {
                if (expression is UnaryExpression)
                    currentThread.AddRange(NextExpression(expression as UnaryExpression, out expression));
                else if (expression is MemberExpression)
                    currentThread.AddRange(NextExpression(expression as MemberExpression, out expression));
                else if (expression is MethodCallExpression)
                    currentThread.AddRange(NextExpression(expression as MethodCallExpression, completedThreads, out expression));
                else if (expression is BinaryExpression)
                    NextExpression(expression as BinaryExpression, completedThreads, out expression);
                else if (expression is ConditionalExpression)
                    NextExpression(expression as ConditionalExpression, completedThreads, out expression);
                else if(expression is ParameterExpression)
                    break;
                else
                {
                    // if expression is ConstantExpression || expression is NewExpression, 
                    // clear because expression does not lead to input parater
                    // else, clear because expression type is not supported
                    currentThread.Clear();
                    break;
                }
            }

            return currentThread.Where(a => a != null).Union(completedThreads).Distinct();
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

        static IEnumerable<MemberInfo> NextExpression(MethodCallExpression ex, List<MemberInfo> completedThreads, out Expression next)
        {
            next = ex.Object;
            if (ex.Arguments != null)
            {
                foreach (var arg in ex.Arguments)
                    completedThreads.AddRange(ReadExpression(arg));
            }

            return new[] { ex.Method };
        }

        static void NextExpression(BinaryExpression ex, List<MemberInfo> completedThreads, out Expression next)
        {
            next = null;
            completedThreads.AddRange(ReadExpression(ex.Left));
            completedThreads.AddRange(ReadExpression(ex.Right));
        }

        static void NextExpression(ConditionalExpression ex, List<MemberInfo> completedThreads, out Expression next)
        {
            next = null;
            completedThreads.AddRange(ReadExpression(ex.Test));
            completedThreads.AddRange(ReadExpression(ex.IfFalse));
            completedThreads.AddRange(ReadExpression(ex.IfTrue));
        }
    }
}
