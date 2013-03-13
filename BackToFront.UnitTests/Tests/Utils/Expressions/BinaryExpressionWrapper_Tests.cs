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
            var ex = Mock.Create<int, bool>(a => a == 0, true);

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
            var ex = Mock.Create<int, bool>(a => a == 0, true);

            // act
            // assert            
            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 0 }));
            Assert.AreEqual(false.ToString(), subject.Evaluate(new object[] { 1 }));

            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 0 }, new[] { ex }));
            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 1 }, new[] { ex }));
        }
    }
}
