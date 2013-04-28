using BackToFront.Expressions;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Expressions.Visitors;

using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions
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

            public Expression _CompileInnerExpression(ISwapPropVisitor mocks)
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
            public string Member { get; set; }
            public string Member2 { get; set; }
        }

        public class TestClass2
        {
            public TestClass Member { get; set; }
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
            Assert.IsTrue(subject.IsSameExpression(exp1));
            Assert.IsTrue(subject.IsSameExpression(exp2));
            Assert.IsFalse(subject.IsSameExpression(exp3));
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var member = Expression.Not(Expression.Parameter(typeof(bool)));
            var subject = new TestSubjectWrapper(member);
            var input = new M.Mock<ISwapPropVisitor>();
            input.Setup(a => a.ContainsNothing).Returns(true);
            input.Setup(a => a.Visit(M.It.IsAny<Expression>())).Returns<Expression>(a => a);

            // act
            var result = subject._CompileInnerExpression(input.Object);

            // assert
            Assert.AreEqual(subject.Expression, result);
        }

        [Test]
        public void CompileInnerExpression_Test_withMocks()
        {
            // arange
            var beforeMock = Expression.Parameter(typeof(bool));
            var afterMock = Expression.Constant(true);
            var testExp = Expression.Not(beforeMock);
            var subject = new TestSubjectWrapper(testExp);
            var input = new M.Mock<ISwapPropVisitor>();
            input.Setup(a => a.ContainsNothing).Returns(false);
            input.Setup(a => a.Visit(M.It.IsAny<Expression>())).Returns(afterMock);

            // act
            var result = subject._CompileInnerExpression(input.Object) as UnaryExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.AreEqual(afterMock, result.Operand);

            Assert.AreEqual(testExp.Method, result.Method);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            var param = Expression.Parameter(typeof(int));
            var expression = Expression.Convert(param, typeof(object));
            var subject = new TestSubjectWrapper(expression);
            var expected = new ParameterExpressionWrapper(param).GetMembersForParameter(param).ElementAt(0);
            // act
            var actual = subject.__GetMembersForParameter(param);

            // assert
            Assert.AreEqual(1, actual.Count());
            Assert.IsTrue(expected.Equals(actual.ElementAt(0)));
        }

        [Test]
        public void UnorderedParameters_Test()
        {
            // arange
            var param1 = Expression.Parameter(typeof(int));

            var mockedExp = Expression.Convert(param1, typeof(double));
            var subject = new UnaryExpressionWrapper(mockedExp);

            // act
            // assert
            Assert.IsTrue(AreKindOfEqual(subject.UnorderedParameters, new[] { param1 }));
        }

        [Test]
        public void WithAlternateRoot_Test()
        {
            // arange
            var constant = new TestClass();
            var before = Expression.Convert(Expression.Parameter(typeof(TestClass)), typeof(object));
            var expected = Expression.Convert(Expression.Constant(constant), typeof(object));

            var subject = new UnaryExpressionWrapper(before);

            // act
            var result = subject.WithAlternateRoot<TestClass, TestClass>(Expression.Constant(constant), null);

            // assert
            Assert.IsTrue(result.IsSameExpression(expected));
        }
    }
}