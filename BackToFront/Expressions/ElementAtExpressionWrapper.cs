//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using E = System.Linq.Expressions;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using BackToFront.Utils;

//using BackToFront.Extensions.IEnumerable;
//using System.Reflection;

//namespace BackToFront.Expressions
//{
//    public class ElementAtExpressionWrapper : MethodCallExpressionWrapper
//    {
//        public static readonly MethodInfo ElementAt;
//        static ElementAtExpressionWrapper()
//        {
//            ElementAt = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(a =>
//            {
//                if (a.Name != "ElementAt")
//                    return false;

//                var generics = a.GetGenericArguments();
//                if (generics.Length != 1)
//                    return false;

//                var paramaters = a.GetParameters();
//                return paramaters.Length == 2 &&
//                    paramaters[0].ParameterType == typeof(IEnumerable<>).MakeGenericType(generics[0]) &&
//                    paramaters[1].ParameterType == typeof(int);
//            });
//        }

//        private readonly bool IsElementAt;

//        public ElementAtExpressionWrapper(MethodCallExpression expression)
//            : base(expression)
//        {
//            IsElementAt = expression.Method == ElementAt;
//        }

//        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
//        {
//            if (IsElementAt)
//                return Object.GetMembersForParameter(parameter).Each(i => i.NextItem = new MemberChainItem(Expression.Method, new MemberIndex()));
//            else
//                return base._GetMembersForParameter(parameter);
//        }
//    }
//}
