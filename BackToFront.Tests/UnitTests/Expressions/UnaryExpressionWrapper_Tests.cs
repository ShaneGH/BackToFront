﻿using BackToFront.Expressions;
using BackToFront.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Tests.UnitTests.Expressions
{
    [TestFixture]
    public class UnaryExpressionWrapper_Tests : Base.TestBase
    {
        public class TestSubjectWrapper : UnaryExpressionWrapper
        {
            public TestSubjectWrapper(UnaryExpression expression)
                : base(expression)
            {
            }

            public Expression _CompileInnerExpression(IEnumerable<Mock> mocks)
            {
                return CompileInnerExpression(mocks);
            }
        }

        public class TestClass
        {
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            var exp1 = Expression.Not(Expression.Parameter(typeof(bool)));
            var exp2 = Expression.Not(Expression.Parameter(typeof(bool)));
            var exp3 = Expression.Convert(Expression.Parameter(typeof(bool)), typeof(object));

            Expression<Func<TestClass, object>> func2 = a => a.ToString();
            Expression<Func<TestClass, object>> func3 = a => a.GetType();
            var subject = new UnaryExpressionWrapper(exp1);

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new UnaryExpressionWrapper(exp1)));
            Assert.IsTrue(subject.IsSameExpression(new UnaryExpressionWrapper(exp2)));
            Assert.IsFalse(subject.IsSameExpression(new UnaryExpressionWrapper(exp3)));
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var member = Expression.Not(Expression.Parameter(typeof(bool)));
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
            var mockedVal = true;
            var mockedExp = Expression.Parameter(typeof(bool));
            var testExp = Expression.Not(mockedExp);
            var subject = new TestSubjectWrapper(testExp);

            // act
            var result = subject._CompileInnerExpression(new[] { new Mock(mockedExp, mockedVal, mockedVal.GetType()) }) as UnaryExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.IsInstanceOf<UnaryExpression>(result.Operand);
            Assert.AreEqual(mockedVal.GetType(), (result.Operand as UnaryExpression).Type);
            Assert.AreEqual(ExpressionType.Convert, result.Operand.NodeType);

            Assert.AreEqual(testExp.Method, result.Method);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
        }
    }
}