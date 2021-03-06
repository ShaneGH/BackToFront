﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;

using NUnit.Framework;
using BackToFront.Framework;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Tests.Base
{
    public class TestBase
    {
        [SetUp]
        public virtual void Setup() { }

        [TearDown]
        public virtual void TearDown() { }

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp() { }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown() { }

        private static MemberInfo[] PrivateMembers(Type type, string name)
        {
            return type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void SetPrivateProperty(object item, string name, object value)
        {
            var members = PrivateMembers(item.GetType(), name).Where(m => m is FieldInfo || m is PropertyInfo);

            if (members.Count() != 1)
                throw new InvalidOperationException("Invalid Property of field name: " + name);

            if (members.ElementAt(0) is FieldInfo)
                (members.ElementAt(0) as FieldInfo).SetValue(item, value);
            else if (members.ElementAt(0) is PropertyInfo)
                (members.ElementAt(0) as PropertyInfo).GetSetMethod(true).Invoke(item, new[]{ value});
            else
                throw new InvalidOperationException("Invalid Property of field name: " + name);
        }

        public static object GetPrivateProperty(object item, string name)
        {
            return GetPrivateProperty(item, name);
        }

        public static object GetPrivateProperty(object item, Type itemType, string name)
        {
            var members = PrivateMembers(itemType, name).Where(m => m is FieldInfo || m is PropertyInfo);

            if (members.Count() != 1)
                throw new InvalidOperationException("Invalid Property of field name: " + name);

            if (members.ElementAt(0) is FieldInfo)
                return (members.ElementAt(0) as FieldInfo).GetValue(item);
            else if (members.ElementAt(0) is PropertyInfo)
                return (members.ElementAt(0) as PropertyInfo).GetValue(item);
            else
                throw new InvalidOperationException("Invalid Property of field name: " + name);
        }

        public static object CallPrivateMethod(object item, string name, IEnumerable<object> args)
        {
            var members = PrivateMembers(item.GetType(), name).Where(m => m is MethodInfo);

            if (members.Count() != 1)
                throw new InvalidOperationException("Invalid Property of field name: " + name);

            if (members.ElementAt(0) is MethodInfo)
                return (members.ElementAt(0) as MethodInfo).Invoke(item, args.ToArray());
            else
                throw new InvalidOperationException("Invalid Property of field name: " + name);
        }

        public static Func<TEntity, TMember> CompileExpression<TEntity, TMember>(Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            return Expression.Lambda<Func<TEntity, TMember>>(expression, parameters).Compile();
        }

        public static bool AreKindOfEqual<T>(IEnumerable<T> item1, IEnumerable<T> item2, Func<T,T,bool> comparitor)
        {
            if (item1 == null && item2 == null)
                return true;

            if (item1 == null || item2 == null || item1.Count() != item2.Count())
                return false;

            for (var i = 0; i < item1.Count(); i++)
                if (!comparitor(item1.ElementAt(i), item2.ElementAt(i)))
                    return false;

            return true;
        }

        public static bool AreKindOfEqual<T>(IEnumerable<T> item1, IEnumerable<T> item2)
        {
            return AreKindOfEqual(item1, item2, (a, b) => a.Equals(b));
        }

        public static void CompileAndCall<TEntity>(Expression logic, ExpressionMocker visitor, TEntity entity, ValidationContext ctxt)
        {
            Expression.Lambda<Action<TEntity, ValidationContext>>(logic, visitor.EntityParameter, visitor.ContextParameter).Compile()(entity, ctxt);
        }
    }
}
