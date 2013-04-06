﻿using BackToFront.Expressions;
using BackToFront.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Tests.UnitTests.Expressions
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

            public Expression _CompileInnerExpression(IEnumerable<Mock> mocks)
            {
                return CompileInnerExpression(mocks);
            }

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
            }
        }

        public class TestClass
        {
            public void Hello(TestClass input) { }
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
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as MethodCallExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as MethodCallExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func3) as MethodCallExpressionWrapper));
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var member = Expression.Call(Expression.Parameter(typeof(object)), typeof(object).GetMethod("ToString"));
            var subject = new TestSubjectWrapper(member);

            // act
            var result = subject._CompileInnerExpression(Enumerable.Empty<Mock>());

            // assert
            Assert.AreEqual(subject.Expression, result);
        }

        [Test]
        public void CompileInnerExpression_Test_withMocks()
        {
            // arange
            var mockedVal = new object();
            var mockedExp = Expression.Parameter(typeof(object));
            var testExp = Expression.Call(mockedExp, typeof(object).GetMethod("ToString"));
            var subject = new TestSubjectWrapper(testExp);

            // act
            var result = subject._CompileInnerExpression(new[] { new Mock(mockedExp, mockedVal, mockedVal.GetType()) }) as MethodCallExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.IsInstanceOf<UnaryExpression>(result.Object);
            Assert.AreEqual(mockedVal.GetType(), (result.Object as UnaryExpression).Type);
            Assert.AreEqual(ExpressionType.Convert, result.Object.NodeType);

            Assert.AreEqual(testExp.Method, result.Method);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
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
    }
}
