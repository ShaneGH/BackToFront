using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utilities;

namespace BackToFront.Tests.CSharp.UnitTests.Utils
{
    [TestFixture]
    public class CompiledMockedExpression_Tests : Base.TestBase
    {
        public class TestClass
        {
            public int Prop { get; set; }
        }

        [Test]
        public void Invoke_Test_AllOk_NoArgs()
        {
            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new CompiledMockedExpression<TestClass, int>((a, b, c) => a.Prop, new Mocks());
            var test = new TestClass { Prop = 54 };

            // act
            var result = subject.Invoke(test, null, null);

            // assert
            Assert.AreEqual(test.Prop, result);
        }

        [Test]
        public void Invoke_Test_AllOk_WithArgs()
        {
            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var mocks = new Mocks(new[] { new Mock(exp.Body, null, typeof(int)), new Mock(exp.Body, null, typeof(int)) });
            var subject = new CompiledMockedExpression<TestClass, int>((a, b, c) => a.Prop, mocks);
            var test = new TestClass { Prop = 54 };

            // act
            var result = subject.Invoke(test, new object[]{ 5, 5 }, null);

            // assert
            Assert.AreEqual(test.Prop, result);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Invoke_Test_Exception_WrongArgAmmount()
        {
            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var mocks = new Mocks(new[] { new Mock(exp.Body, null, typeof(int)), new Mock(exp.Body, null, typeof(int)) });
            var subject = new CompiledMockedExpression<TestClass, int>((a, b, c) => a.Prop, mocks);
            var test = new TestClass { Prop = 54 };

            // act
            // assert
            var result = subject.Invoke(test, new object[] { 5, 5, 5 }, null);
        }
    }
}