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
    public class MethodCallExpressionWrapper_Tests : Base.TestBase
    {
        public class TestClass
        {
            public static int staticCount = 2;

            public readonly int count;

            public TestClass()
            {
                count = ++staticCount;
            }

            public string aRandomString()
            {
                StringBuilder b = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    b.Append("A");
                }

                return b.ToString();
            }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<TestClass, object>> func1 = a => a.ToString();
            Expression<Func<TestClass, object>> func2 = a => a.ToString();
            Expression<Func<TestClass, object>> func3 = a => a.GetType();
            var subject = new FuncExpressionWrapper<TestClass, object>(func1).Body as MethodCallExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<TestClass, object>(func1).Body as MethodCallExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<TestClass, object>(func2).Body as MethodCallExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<TestClass, object>(func3).Body as MethodCallExpressionWrapper));
        }

        [Test]
        public void EvaluateTest()
        {
            // arange
            Expression<Func<TestClass, object>> func1 = a => a.GetHashCode();
            var subject = new FuncExpressionWrapper<TestClass, object>(func1);
            var input1 = new TestClass();
            var hash = input1.GetHashCode();
            var ex = Mock.Create<TestClass, object>(a => a.GetHashCode(), hash + 1);

            // act
            // assert            
            Assert.AreEqual(hash, subject.Evaluate(new[] { input1 }));
            Assert.AreEqual(hash + 1, subject.Evaluate(new[] { input1 }, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest2()
        {
            // arange
            Expression<Func<TestClass, int>> func1 = a => a.GetHashCode().GetHashCode();
            var subject = new FuncExpressionWrapper<TestClass, int>(func1);
            var input1 = new TestClass();
            var ex = Mock.Create<TestClass, int>(a => a.GetHashCode(), input1.GetHashCode() + 1);

            // act
            // assert            
            Assert.AreEqual(input1.GetHashCode().GetHashCode(), subject.Evaluate(new[] { input1 }));
            Assert.AreEqual((input1.GetHashCode() + 1).GetHashCode(), subject.Evaluate(new[] { input1 }, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest1()
        {
            const string test = "ldkhasldkhaslkdhlaskhdlakshdsalkhdlakshdlkashd";

            // arange
            Expression<Func<TestClass, int>> func1 = a => a.aRandomString().Length;
            var subject = new FuncExpressionWrapper<TestClass, int>(func1);
            var input1 = new TestClass();
            var ex = Mock.Create<TestClass, object>(a => a.aRandomString(), test);

            // act
            // assert            
            Assert.AreEqual(input1.aRandomString().Length, subject.Evaluate(new[] { input1 }));
            Assert.AreEqual(test.Length, subject.Evaluate(new[] { input1 }, new[] { ex }));
        }
    }
}
