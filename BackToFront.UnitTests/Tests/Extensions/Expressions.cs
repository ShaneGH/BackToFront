using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Expressions;

using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Extensions
{
    [TestFixture]
    public class Expressions_Tests
    {
        class TestClass
        {
            public string Something { get; set; }
            public string Something2 { get; set; }
            public string Something3 { get; set; }
            public bool Something4 { get; set; }

            public object Method1() { return null; }
            public object Method2(string input) { return null; }
        }

        [Test]
        public void NextExpression_New_Test()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => new TestClass().Something.Length;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void NextExpression_Constant_Test()
        {
            // arrange
            var test = new TestClass();
            Expression<Func<TestClass, object>> subject = a => test.Something.Length;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        [Ignore]
        public void NextExpression_Unary_Test()
        {
            // test this?
        }

        [Test]
        public void NextExpression_Member_Test()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Something.Length;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(typeof(string).GetMember("Length").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
        }

        [Test]
        public void NextExpression_MethodCall_Test()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Method1();

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Method1").Single()));
        }

        [Test]
        public void NextExpression_Inverted_MethodCall_Test()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => "Hello".Equals(a.Something3);

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something3").Single()));
        }

        [Test]
        public void NextExpression_MethodCall_Test_ConstArgs()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Method2("hi");

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Method2").Single()));
        }

        [Test]
        public void NextExpression_MethodCall_Test_MemberArgs()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Method2(a.Something);

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Method2").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
        }

        [Test]
        public void NextExpression_Binary_Test_2Side()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Something == "Hi";

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
        }

        [Test]
        public void NextExpression_Binary_Test_2Sides()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Something == a.Something2;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something2").Single()));
        }

        [Test]
        public void NextExpression_Conditional_Test_NoSides()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => true ? "Hi" : "Hello";

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void NextExpression_Conditional_Test_3Sides()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Something4 ? a.Something : a.Something2;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something2").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something4").Single()));
        }

        [Test]
        public void NextExpression_Conditional_Test_MissingLhs()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Something4 ? "Hi" : a.Something2;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something2").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something4").Single()));
        }

        [Test]
        public void NextExpression_Conditional_Test_MissingRhs()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => a.Something4 ? a.Something : "hi";

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something4").Single()));
        }

        [Test]
        public void NextExpression_Conditional_Test_MissingCondition()
        {
            // arrange
            Expression<Func<TestClass, object>> subject = a => true ? a.Something : a.Something2;

            // act
            var result = subject.ReferencedProperties();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something").Single()));
            Assert.IsTrue(result.Contains(typeof(TestClass).GetMember("Something2").Single()));
        }
    }
}
