﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace BackToFront.Tests.UnitTests.Expressions
{
    public class TestSubjectWrapper : ParameterExpressionWrapper
    {
        public TestSubjectWrapper(ParameterExpression expression)
            : base(expression)
        {
        }

        public Expression _CompileInnerExpression(IEnumerable<Mock> mocks)
        {
            return CompileInnerExpression(mocks);
        }
    }

    [TestFixture]
    public class ParameterExpressionWrapper_Tests : Base.TestBase
    {
        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<object, object>> func1 = a => a;
            Expression<Func<object, object>> func2 = a => a;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as ParameterExpressionWrapper;

            // act
            var test1 = subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as ParameterExpressionWrapper);
            var test2 = subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as ParameterExpressionWrapper);

            // assert
            Assert.IsTrue(test1);
            Assert.IsTrue(test2);
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var member = Expression.Parameter(typeof(string));
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
            var member = Expression.Parameter(typeof(string));
            var subject = new TestSubjectWrapper(member);

            // act
            var result = subject._CompileInnerExpression(Enumerable.Empty<Mock>());

            // assert
            Assert.AreEqual(subject.Expression, result);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetTest_InvalidRoot()
        {
            // arange
            var member = Expression.Parameter(typeof(string));
            var subject = new TestSubjectWrapper(member);

            // act
            // assert
            var result = subject.Get(3);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetTest_InvalidRoot_ProvideParentInstance()
        {
            // arange
            var member = Expression.Parameter(typeof(string));
            var subject = new TestSubjectWrapper(member);

            // act
            // assert
            var result = subject.Get(new object());
        }

        [Test]
        public void GetTest_ProvideCorrectInstance()
        {
            // arange
            var expected = "KJBJB";
            var member = Expression.Parameter(typeof(string));
            var subject = new TestSubjectWrapper(member);

            // act
            var result = subject.Get(expected);

            // assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetTest_ProvideChildInstance()
        {
            // arange
            var expected = "KJBJB";
            var member = Expression.Parameter(typeof(object));
            var subject = new TestSubjectWrapper(member);

            // act
            var result = subject.Get(expected);

            // assert
            Assert.AreEqual(expected, result);
        }
    }
}