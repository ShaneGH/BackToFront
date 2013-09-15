using BackToFront.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions
{
    [TestFixture]
    public class MethodCallExpressionWrapper_Tests : Base.TestBase
    {
        public class TestSubjectWrapper : MethodCallExpressionWrapper
        {
            public TestSubjectWrapper(MethodCallExpression expression)
                : base(expression)
            {
            }

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
            }
        }

        public class TestClass
        {
            public int ChildProp { get; set; }

            public void Hello(TestClass input) { }

            public bool HelloMethod(int val)
            {
                return true;
            }

            public static bool HelloO() { return true; }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, object>> func1 = a => a.ToString();
            Expression<Func<TestClass, object>> func2 = a => a.ToString();
            Expression<Func<TestClass, object>> func3 = a => a.GetType();
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters) as MethodCallExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(func1.Body));
            Assert.IsTrue(subject.IsSameExpression(func2.Body));
            Assert.IsFalse(subject.IsSameExpression(func3.Body));
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            TestClass mockedVal = new TestClass();
            var param = Expression.Parameter(typeof(TestClass));
            var member = typeof(TestClass).GetMethod("Hello");
            var testExp = Expression.Call(param, member, param);
            var subject = new TestSubjectWrapper(testExp);

            // act
            var actual = subject.__GetMembersForParameter(param);
            var expected = new ParameterExpressionWrapper(param).GetMembersForParameter(param).ElementAt(0);

            // assert
            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(actual.ElementAt(0).Member, expected.Member);
            Assert.NotNull(actual.ElementAt(0).NextItem);
            Assert.AreEqual(actual.ElementAt(0).NextItem.Member, member);
            Assert.IsNull(actual.ElementAt(0).NextItem.NextItem);

            var tmp = actual.ElementAt(1);
            Assert.IsTrue(tmp.Equals(expected));
        }

        [Test]
        public void UnorderedParameters_Test()
        {
            // arange
            var param1 = Expression.Parameter(typeof(string));
            var param2 = Expression.Parameter(typeof(IFormatProvider));

            var mockedExp = Expression.Call(param1, typeof(string).GetMethod("ToString", new[] { typeof(IFormatProvider) }), param2);
            var subject = new MethodCallExpressionWrapper(mockedExp);

            // act
            // assert
            Assert.IsTrue(AreKindOfEqual(subject.UnorderedParameters, new[] { param1, param2 }));
        }

        [Test]
        public void WithAlternateRoot_Test_WithoutMethodParams()
        {
            // arrnage
            var helloMethod = typeof(TestClass).GetMethod("HelloMethod");
            var hashMethod = typeof(TestClass).GetMethod("GetHashCode");
            var constant = Expression.Constant(new TestClass());
            var subject = (MethodCallExpressionWrapper)ExpressionWrapperBase.ToWrapper<TestClass, bool>(a => a.HelloMethod(4));

            // act
            var result = subject.WithAlternateRoot<TestClass, TestClass>(constant, a => a);

            // assert
            Assert.IsTrue(result.IsSameExpression(Expression.Call(constant, helloMethod, Expression.Constant(4))));
        }

        [Test]
        public void WithAlternateRoot_Test_WithMethodParams()
        {
            // arrnage
            var helloMethod = typeof(TestClass).GetMethod("HelloMethod");
            var hashMethod = typeof(TestClass).GetMethod("GetHashCode");
            var constant = Expression.Constant(new TestClass());
            var subject = (MethodCallExpressionWrapper)ExpressionWrapperBase.ToWrapper<TestClass, bool>(a => a.HelloMethod(a.GetHashCode()));

            // act
            var result = subject.WithAlternateRoot<TestClass, TestClass>(constant, a => a);

            // assert
            Assert.IsTrue(result.IsSameExpression(Expression.Call(constant, helloMethod, Expression.Call(constant, hashMethod))));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithAlternateRoot_Test_MethodArgumentCannotBeMocked()
        {
            // arrnage
            var constant = Expression.Constant(new TestClass());
            var subject = (MethodCallExpressionWrapper)ExpressionWrapperBase.ToWrapper<TestClass, bool>(a => a.HelloMethod(a.GetHashCode()));

            // act
            // assert
            subject.WithAlternateRoot<TestClass, int>(constant, a => a.ChildProp);
        }

        [Test]
        public void IsStatic_Test_NotStatic()
        {
            // arrnage
            var constant = Expression.Constant(new TestClass());
            var subject = (MethodCallExpressionWrapper)ExpressionWrapperBase.ToWrapper<TestClass, bool>(a => a.HelloMethod(a.GetHashCode()));

            // act
            var actual = subject.IsStatic;

            // assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsStatic_Test_Static()
        {
            // arrnage
            var subject = (MethodCallExpressionWrapper)ExpressionWrapperBase.ToWrapper<TestClass, bool>(a => TestClass.HelloO());

            // act
            var actual = subject.IsStatic;

            // assert
            Assert.IsTrue(actual);
        }
    }
}
