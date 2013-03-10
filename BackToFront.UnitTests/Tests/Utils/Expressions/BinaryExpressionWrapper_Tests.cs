using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.UnitTests.Utilities;
using BackToFront.Utils.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Utils.Expressions
{
    [TestFixture]
    public class BinaryExpressionWrapper_Tests : Base.TestBase
    {
        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<int, bool>> func1 = a => a == 4;
            Expression<Func<int, bool>> func2 = a => a == 4;
            Expression<Func<int, bool>> func3 = a => a == 5;
            Expression<Func<int, bool>> func4 = a => a != 4;
            var subject = new FuncExpressionWrapper<int, bool>(func1).Body as BinaryExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func1).Body as BinaryExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func2).Body as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func3).Body as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func4).Body as BinaryExpressionWrapper));
        }

        [Test]
        public void EvaluateTest()
        {
            // arange
            var subject = new FuncExpressionWrapper<int, bool>(a => a == 0);
            var ex = new Tuple<Expression<Func<int, bool>>, bool>(a => a == 0, true);

            // act
            // assert            
            Assert.IsTrue((bool)subject.Evaluate(new object[] { 0 }));
            Assert.IsFalse((bool)subject.Evaluate(new object[] { 1 }));

            Assert.IsTrue((bool)subject.Evaluate(new object[] { 0 }, new []{ ex }));
            Assert.IsTrue((bool)subject.Evaluate(new object[] { 1 }, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest()
        {
            // arange
            var subject = new FuncExpressionWrapper<int, string>(a => (a == 0).ToString());
            var ex = new Tuple<Expression<Func<int, bool>>, bool>(a => a == 0, true);

            // act
            // assert            
            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 0 }));
            Assert.AreEqual(false.ToString(), subject.Evaluate(new object[] { 1 }));

            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 0 }, new[] { ex }));
            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 1 }, new[] { ex }));
        }

        #region operators

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void OperatorTest_AndAlso(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a && b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_OrElse(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a || b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_Or(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a || b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void OperatorTest_Equal(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a == b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_NotEqual(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a != b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        public void OperatorTest_Add()
        {
            var subject = new FuncExpressionWrapper<int, int, int>((a, b) => a + b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(5, subject.Evaluate(new object[] { 3, 2 }));
        }

        [Test]
        public void OperatorTest_Subtract()
        {
            var subject = new FuncExpressionWrapper<int, int, int>((a, b) => a - b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(1, subject.Evaluate(new object[] { 3, 2 }));
        }

        #endregion
    }
}
