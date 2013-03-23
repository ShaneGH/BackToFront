using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace BackToFront.UnitTests.Tests.Expressions
{
    [TestFixture]
    public class MemberExpressionWrapper_Tests : Base.TestBase
    {
        public class TestSubjectWrapper : MemberExpressionWrapper
        {
            public TestSubjectWrapper(MemberExpression expression)
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
            public string Member { get; set; }
            public string Member2 { get; set; }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<TestClass, object>> func1 = a => a.Member;
            Expression<Func<TestClass, object>> func2 = a => a.Member;
            Expression<Func<TestClass, object>> func3 = a => a.Member2;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as MemberExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as MemberExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as MemberExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func3) as MemberExpressionWrapper));
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var member = Expression.Property(Expression.Parameter(typeof(TestClass)), "Member");
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
            TestClass mockedVal = new TestClass();
            var mockedExp = Expression.Parameter(typeof(TestClass));
            var testExp = Expression.Property(mockedExp, "Member");
            var subject = new TestSubjectWrapper(testExp);

            // act
            var result = subject._CompileInnerExpression(new[] { new Mock(mockedExp, mockedVal) }) as MemberExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.IsInstanceOf<ConstantExpression>(result.Expression);
            Assert.AreEqual(mockedVal, (result.Expression as ConstantExpression).Value);
            Assert.AreEqual(testExp.Member, result.Member);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
        }
    }
}
