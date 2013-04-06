using System;
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
            var result = subject._CompileInnerExpression(new[] { new Mock(mockedExp, mockedVal, mockedVal.GetType()) }) as MemberExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.IsInstanceOf<UnaryExpression>(result.Expression);
            Assert.AreEqual(mockedVal.GetType(), (result.Expression as UnaryExpression).Type);
            Assert.AreEqual(ExpressionType.Convert, result.Expression.NodeType);

            Assert.AreEqual(testExp.Member, result.Member);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            TestClass mockedVal = new TestClass();
            var mockedExp = Expression.Parameter(typeof(TestClass));
            var member = typeof(TestClass).GetProperty("Member");
            var testExp = Expression.Property(mockedExp, member);
            var subject = new TestSubjectWrapper(testExp);

            // act
            var result = subject.__GetMembersForParameter(mockedExp);

            // assert
            Assert.AreEqual(1, result.Count());
            var test = result.ElementAt(0);

            Assert.AreEqual(typeof(TestClass), test.Member);
            Assert.NotNull(test.NextItem);
            Assert.AreEqual(member, test.NextItem.Member);
            Assert.IsNull(test.NextItem.NextItem);
        }

        [Test]
        public void UnorderedParameters_Test()
        {
            // arange
            var mockedExp = Expression.Parameter(typeof(TestClass));
            var member = typeof(TestClass).GetProperty("Member");
            var testExp = Expression.Property(mockedExp, member);
            var subject = new TestSubjectWrapper(testExp);

            // act
            // assert
            Assert.AreEqual(1, subject.UnorderedParameters.Count());
            Assert.AreEqual(mockedExp, subject.UnorderedParameters.First());
        }
    }
}
