using BackToFront.Expressions;
using BackToFront.Utilities;
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

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
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
    }
}
