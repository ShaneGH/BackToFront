using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;
using BackToFront.Expressions.Visitors;
using BackToFront.Dependency;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions
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
            Expression<Func<TestClass, object>> func1 = a => a.Member;
            Expression<Func<TestClass, object>> func2 = a => a.Member;
            Expression<Func<TestClass, object>> func3 = a => a.Member2;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as MemberExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(func1.Body));
            Assert.IsTrue(subject.IsSameExpression(func2.Body));
            Assert.IsFalse(subject.IsSameExpression(func3.Body));
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var member = Expression.Property(Expression.Parameter(typeof(TestClass)), "Member");
            var subject = new TestSubjectWrapper(member);

            // act
            var result = subject._CompileInnerExpression(new SwapPropVisitor());

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
            var result = subject._CompileInnerExpression(new SwapPropVisitor(new Mocks(new[] { new Mock(mockedExp, mockedVal, mockedVal.GetType()) }), new Dependencies())) as MemberExpression;

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

        [Test]
        public void WithAlternateRoot_Test()
        {
            // arange
            var constant = new TestClass();
            var mockedExp = Expression.Parameter(typeof(TestClass2));
            var intermediary = Expression.Property(mockedExp, typeof(TestClass2).GetProperty("Member"));
            var testExp = Expression.Property(intermediary, typeof(TestClass).GetProperty("Member"));
            var subject = new TestSubjectWrapper(testExp);

            var expected = Expression.Property(Expression.Constant(constant), "Member");

            // act
            var result = subject.WithAlternateRoot<TestClass2, TestClass>(Expression.Constant(constant), null);
            var ttt = ((MemberExpression)result.WrappedExpression).Expression.GetType();

            // assert
            Assert.IsTrue(result.IsSameExpression(expected));
        }
    }
}
