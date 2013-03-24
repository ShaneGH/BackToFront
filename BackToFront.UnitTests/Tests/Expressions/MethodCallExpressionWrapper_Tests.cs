using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using System.Collections.ObjectModel;
using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Expressions
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
        }

        public class TestClass
        {
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
    }
}
