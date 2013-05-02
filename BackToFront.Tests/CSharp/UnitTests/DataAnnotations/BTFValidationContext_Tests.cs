using BackToFront.DataAnnotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.DataAnnotations
{
    [TestFixture]
    public class BTFValidationContext_Tests : Base.TestBase
    {
        private class TestClass1 : object { }
        private class TestClass2 : TestClass1 { }

        [Test]
        public void Constructor_Test_Multiple_RulesAndClasses()
        {
            // arrange
            var repository = new Repository();
            repository.AddRule<TestClass1, ITestAction>((a, something) => { });
            repository.AddRule<TestClass2>(a => { });
            repository.AddRule<TestClass2>(a => { });

            var vc = new ValidationContext(new TestClass2());
            vc.ServiceContainer.AddService(typeof(Repository), repository);

            // act
            var subject = new BTFValidationContext(vc);
            
            // assert
            Assert.AreEqual(3, subject.Rules.Length);
            Assert.AreEqual(repository.Rules<TestClass2>().ElementAt(0), subject.Rules[0].Rule);
            Assert.AreEqual(repository.Rules<TestClass2>().ElementAt(1), subject.Rules[1].Rule);
            Assert.AreEqual(repository.Rules<TestClass1>().ElementAt(0), subject.Rules[2].Rule);
        }
    }
}