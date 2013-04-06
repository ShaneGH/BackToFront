using BackToFront.DataAnnotations;
using NUnit.Framework;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using M = Moq;
using BackToFront.Dependency;
using System.Collections.Generic;

namespace BackToFront.Tests.UnitTests.DataAnnotations
{
    [TestFixture]
    public class BTFValidationContext_Tests : Base.TestBase
    {
        private class TestClass1 : object { }
        private class TestClass2 : TestClass1 { }

        static BTFValidationContext_Tests()
        {
            Rules<TestClass1>.AddRule<ITestAction>((a, something) => { });

            Rules<TestClass2>.AddRule(a => { });
            Rules<TestClass2>.AddRule(a => { });
        }

        private class TestException : Exception
        { }

        [Test]
        public void Constructor_Test_Multiple_RulesAndClasses()
        {
            // arrange
            var item = new TestClass2();

            // act
            var subject = new BTFValidationContext(new ValidationContext(item));

            // assert
            Assert.AreEqual(3, subject.Rules.Length);
            Assert.AreEqual(Rules<TestClass2>.Repository.Registered.ElementAt(0), subject.Rules[0].Rule.Item);
            Assert.AreEqual(Rules<TestClass2>.Repository.Registered.ElementAt(1), subject.Rules[1].Rule.Item);
            Assert.AreEqual(Rules<TestClass1>.Repository.Registered.ElementAt(0), subject.Rules[2].Rule.Item);
        }
    }
}