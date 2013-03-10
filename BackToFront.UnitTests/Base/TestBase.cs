using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NUnit.Framework;

namespace BackToFront.UnitTests.Base
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
            var members = PrivateMembers(item.GetType(), name).Where(m => m is FieldInfo || m is PropertyInfo);

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
    }
}
