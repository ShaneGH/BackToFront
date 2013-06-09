using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using BackToFront.Expressions.Visitors;

using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions
{
    [TestFixture]
    public class InvocationExpressionWrapper_Tests : Base.TestBase
    {
        public class TestClass
        {
            public bool Prop { get; set; }

            public bool Prop2 { get; set; }

            public string Method() { return null; }
        }

        public class Accessor : InvocationExpressionWrapper
        {
            public Accessor(InvocationExpression expression)
                : base(expression)
            {
            }

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
            }

            public IEnumerable<ParameterExpression> __UnorderedParameters
            {
                get
                {
                    return _UnorderedParameters;
                }
            }
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arrange
            var method = typeof(TestClass).GetMethod("Method");
            var prop = typeof(TestClass).GetProperty("Prop");
            Action<bool, bool, string> toInvoke = (a, b, c) => { };
            Expression<Func<TestClass, bool>> func1 = a => a.Prop;
            Expression<Func<TestClass, bool>> func2 = a => a.Prop2;
            Expression func3 = Expression.Call(func1.Parameters[0], method);

            var subject = new Accessor(Expression.Invoke(Expression.Constant(toInvoke), func1.Body, func2.Body, func3));

            var expected = new[] 
            {
                new MemberChainItem(typeof(TestClass))
                {
                    NextItem = new MemberChainItem(prop)
                },
                new MemberChainItem(typeof(TestClass))
                {
                    NextItem = new MemberChainItem(method)
                }
            };

            // act
            var actual = subject.__GetMembersForParameter(func1.Parameters[0]);

            // assert
            Assert.AreEqual(expected, actual.ToArray());
        }

        [Test]
        public void _UnorderedParameters_Test()
        {
            // arrange
            var method = typeof(TestClass).GetMethod("Method");
            var prop = typeof(TestClass).GetProperty("Prop");
            Action<bool, bool, string> toInvoke = (a, b, c) => { };
            Expression<Func<TestClass, bool>> func1 = a => a.Prop;
            Expression<Func<TestClass, bool>> func2 = a => a.Prop2;
            Expression func3 = Expression.Call(func1.Parameters[0], method);

            var subject = new Accessor(Expression.Invoke(Expression.Constant(toInvoke), func1.Body, func2.Body, func3));

            var expected = new[] { func1.Parameters[0], func2.Parameters[0] };

            // act
            var actual = subject.__UnorderedParameters;

            // assert
            Assert.AreEqual(expected, actual.ToArray());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsSameExpression_Test(bool isSame)
        {
            // arrange
            var method = typeof(TestClass).GetMethod("Method");
            var prop = typeof(TestClass).GetProperty("Prop");
            Action<bool, bool, string> toInvoke = (a, b, c) => { };
            Expression<Func<TestClass, bool>> func1 = a => a.Prop;
            Expression<Func<TestClass, bool>> func2 = a => a.Prop2;
            Expression func3 = Expression.Call(func1.Parameters[0], method);

            Func<bool, InvocationExpression> create = a => a ? 
                Expression.Invoke(Expression.Constant(toInvoke), func1.Body, func2.Body, func3) : 
                Expression.Invoke(Expression.Constant(toInvoke), Expression.Constant(true), func2.Body, func3);
            var subject = new Accessor(create(true));

            // act
            var actual = subject.IsSameExpression(create(isSame));

            // assert
            Assert.AreEqual(isSame, actual);
        }
    }
}