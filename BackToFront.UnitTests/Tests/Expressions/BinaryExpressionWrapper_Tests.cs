﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Expressions
{
    [TestFixture]
    public class BinaryExpressionWrapper_Tests : Base.TestBase
    {
        public class TestClass : BinaryExpressionWrapper
        {
            public TestClass(BinaryExpression expression)
                : base(expression)
            {
            }

            public Expression _Compile(IEnumerable<Mock> mocks)
            {
                return OnCompile(mocks);
            }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<int, bool>> func1 = a => a == 4;
            Expression<Func<int, bool>> func2 = a => a == 4;
            Expression<Func<int, bool>> func3 = a => a == 5;
            Expression<Func<int, bool>> func4 = a => a != 4;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as BinaryExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as BinaryExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func3) as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func4) as BinaryExpressionWrapper));
        }

        [Test]
        public void OnEvaluate_Test_nothing_mocked()
        {
            // arange
            var subject = new TestClass(
                Expression.Add(Expression.Constant(2), Expression.Constant(3)));

            // act
            var result = subject._Compile(Enumerable.Empty<Mock>());

            // assert
            Assert.AreEqual(subject.Expression, result);
        }

        [Test]
        public void OnEvaluate_Test_withMocks()
        {
            const int mockedVal = 99;

            // arange
            var mockedExp = Expression.Add(Expression.Constant(4), Expression.Constant(2));
            var testExp = Expression.Add(mockedExp, Expression.Constant(3));
            var subject = new TestClass(testExp);

            // act
            var result = subject._Compile(new[] { new Mock(mockedExp, mockedVal) }) as BinaryExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.IsInstanceOf<ConstantExpression>(result.Left);
            Assert.AreEqual(mockedVal, (result.Left as ConstantExpression).Value);
            Assert.AreEqual(testExp.Right, result.Right);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
            Assert.AreEqual(testExp.Method, result.Method);
        }
    }
}
