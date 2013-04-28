using BackToFront.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using M = Moq;

namespace BackToFront.Tests.UnitTests.Expressions
{
    [TestFixture]
    public class ConstantExpressionWrapper_Tests : Base.TestBase
    {
        public class TestClass : ConstantExpressionWrapper
        {
            public TestClass(ConstantExpression expression)
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

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<object, int>> func1 = a => 4;
            Expression<Func<object, int>> func2 = a => 4;
            Expression<Func<object, int>> func3 = a => 5;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as ConstantExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(func1.Body));
            Assert.IsTrue(subject.IsSameExpression(func2.Body));
            Assert.IsFalse(subject.IsSameExpression(func3.Body));
        }

        [Test]
        public void CompileInnerExpression_Test()
        {
            // arange
            var subject = new TestClass(Expression.Constant(4));

            // act
            var result = subject._CompileInnerExpression(new M.Mock<ISwapPropVisitor>().Object);

            // assert
            Assert.AreEqual(subject.Expression, result);
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            var subject = new TestClass(Expression.Constant(4));

            // act
            var result = subject.__GetMembersForParameter(null);

            // assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void UnorderedParameters_Test()
        {
            // arange
            var subject = new TestClass(Expression.Constant(4));

            // act
            var result = subject.UnorderedParameters;

            // assert
            Assert.AreEqual(0, result.Count());
        }
    }
}
