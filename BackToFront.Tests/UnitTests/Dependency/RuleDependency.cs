using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Dependency;
using NUnit.Framework;
using BackToFront.Enum;
using BackToFront.Expressions;

namespace BackToFront.Tests.UnitTests.Dependency
{
    [TestFixture]
    public class RuleDependency_Tests : Base.TestBase
    {
        [Test]
        public void ToMockTest()
        {
            // arrange
            var subject = new RuleDependency("UGBOIUG", new object());

            // act
            var mock = subject.ToMock();

            // assert
            Assert.AreEqual(MockBehavior.MockOnly, mock.Behavior);
            Assert.IsInstanceOf<ConstantExpressionWrapper>(mock.Expression);
            Assert.AreEqual(subject, ((ConstantExpressionWrapper)mock.Expression).Expression.Value);
            Assert.AreEqual(subject.Value, mock.Value);
            Assert.AreEqual(typeof(object), mock.ValueType);
        }
    }
}
