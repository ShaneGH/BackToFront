using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Utils.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace BackToFront.UnitTests.Tests.Utils.Expressions
{
    [TestFixture]
    public class MemberExpressionWrapper_Tests : Base.TestBase
    {
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
        public void EvaluateTest()
        {
            const string mock = "Not hello";

            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, object>> func1 = a => a.Member;
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters);
            var input1 = new TestClass { Member = "Hello" };
            var ex = Mock.Create<TestClass, object>(a => a.Member, mock);

            // act
            // assert            
            Assert.AreEqual(input1.Member, subject.CompileAndCall<TestClass, object>(parameters, input1));
            Assert.AreEqual(mock, subject.CompileAndCall<TestClass, object>(parameters, input1, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest()
        {
            const string mock = "Not hello";

            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, object>> func1 = a => a.Member.GetHashCode();
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters);
            var input1 = new TestClass { Member = "Hello" };
            var ex = Mock.Create<TestClass, object>(a => a.Member, mock);

            // act
            // assert            
            Assert.AreEqual(input1.Member.GetHashCode(), subject.CompileAndCall<TestClass, object>(parameters, input1));
            Assert.AreEqual(mock.GetHashCode(), subject.CompileAndCall<TestClass, object>(parameters, input1, new[] { ex }));
        }

        [Test]
        public void Shallow_EvaluateTest()
        {
            const int mock = 334455;

            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, int>> func1 = a => a.ToString().Length;
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters);
            var input1 = new TestClass { Member = "Hello" };
            var ex = Mock.Create<TestClass, int>(a => a.ToString().Length, mock);

            // act
            // assert            
            Assert.AreEqual(input1.ToString().Length, subject.CompileAndCall<TestClass, int>(parameters, input1));
            Assert.AreEqual(mock, subject.CompileAndCall<TestClass, int>(parameters, input1, new[] { ex }));
        }
    }
}
