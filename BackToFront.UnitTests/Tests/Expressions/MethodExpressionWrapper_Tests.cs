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
        public void EvaluateTest()
        {
            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, int>> func1 = a => a.GetHashCode();
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters);
            var input1 = new TestClass();
            var hash = input1.GetHashCode();
            var ex = Mock.Create<TestClass, int>(a => a.GetHashCode(), hash + 1);

            // act
            // assert            
            Assert.AreEqual(hash, subject.CompileAndCall<TestClass, int>(parameters, input1));
            Assert.AreEqual(hash + 1, subject.CompileAndCall<TestClass, int>(parameters, input1, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest2()
        {
            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, int>> func1 = a => a.GetHashCode().GetHashCode();
            var subject = ExpressionWrapperBase.ToWrapper<TestClass, int>(func1, out parameters);
            var input1 = new TestClass();
            var ex = Mock.Create<TestClass, int>(a => a.GetHashCode(), input1.GetHashCode() + 1);

            // act
            // assert            
            Assert.AreEqual(input1.GetHashCode().GetHashCode(), subject.CompileAndCall<TestClass, int>(parameters, input1));
            Assert.AreEqual((input1.GetHashCode() + 1).GetHashCode(), subject.CompileAndCall<TestClass, int>(parameters, input1, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest1()
        {
            const string test = "ldkhasldkhaslkdhlaskhdlakshdsalkhdlakshdlkashd";

            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<TestClass, int>> func1 = a => a.aRandomString().Length;
            var subject = ExpressionWrapperBase.ToWrapper<TestClass, int>(func1, out parameters);
            var input1 = new TestClass();
            var ex = Mock.Create<TestClass, object>(a => a.aRandomString(), test);

            // act
            // assert            
            Assert.AreEqual(input1.aRandomString().Length, subject.CompileAndCall<TestClass, int>(parameters, input1));
            Assert.AreEqual(test.Length, subject.CompileAndCall<TestClass, int>(parameters, input1, new[] { ex }));
        }
    }
}
