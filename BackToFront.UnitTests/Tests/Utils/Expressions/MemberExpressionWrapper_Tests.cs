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
            var subject = new FuncExpressionWrapper<TestClass, object>(func1).Body as MemberExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<TestClass, object>(func1).Body as MemberExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<TestClass, object>(func2).Body as MemberExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<TestClass, object>(func3).Body as MemberExpressionWrapper));
        }

        [Test]
        public void EvaluateTest()
        {
            const string mock = "Not hello";

            // arange
            Expression<Func<TestClass, object>> func1 = a => a.Member;
            var subject = new FuncExpressionWrapper<TestClass, object>(func1);
            var input1 = new TestClass { Member = "Hello" };
            var ex = Mock.Create<TestClass, object>(a => a.Member, mock);

            // act
            // assert            
            Assert.AreEqual(input1.Member, subject.Evaluate(new[] { input1 }));
            Assert.AreEqual(mock, subject.Evaluate(new[] { input1 }, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest()
        {
            const string mock = "Not hello";

            // arange
            Expression<Func<TestClass, object>> func1 = a => a.Member.GetHashCode();
            var subject = new FuncExpressionWrapper<TestClass, object>(func1);
            var input1 = new TestClass { Member = "Hello" };
            var ex = Mock.Create<TestClass, object>(a => a.Member, mock);

            // act
            // assert            
            Assert.AreEqual(input1.Member.GetHashCode(), subject.Evaluate(new[] { input1 }));
            Assert.AreEqual(mock.GetHashCode(), subject.Evaluate(new[] { input1 }, new[] { ex }));
        }

        [Test]
        public void Shallow_EvaluateTest()
        {
            const int mock = 334455;

            // arange
            Expression<Func<TestClass, object>> func1 = a => a.ToString().Length;
            var subject = new FuncExpressionWrapper<TestClass, object>(func1);
            var input1 = new TestClass { Member = "Hello" };
            var ex = Mock.Create<TestClass, object>(a => a.ToString().Length, mock);

            // act
            // assert            
            Assert.AreEqual(input1.ToString().Length, subject.Evaluate(new[] { input1 }));
            Assert.AreEqual(mock, subject.Evaluate(new[] { input1 }, new[] { ex }));
        }
    }
}
