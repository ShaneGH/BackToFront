using BackToFront.DataAnnotations;
using BackToFront.Dependency;
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
            var di = new M.Mock<IRuleDependencies>();
            var objectInstance = new TestClass2();
            var repository = new Repository();
            repository.AddRule<TestClass1, ITestAction>((a, something) => { });
            repository.AddRule<TestClass2>(a => { });
            repository.AddRule<TestClass2>(a => { });

            var vc = new ValidationContext(objectInstance);
            vc.ServiceContainer.AddService(typeof(IRuleDependencies), di.Object);

            // act
            var subject = new BTFValidationContext(vc, repository);
            
            // assert
            Assert.AreEqual(3, subject.Rules.Length);
            Assert.AreEqual(repository.Rules<TestClass2>().ElementAt(0), subject.Rules[0]);
            Assert.AreEqual(repository.Rules<TestClass2>().ElementAt(1), subject.Rules[1]);
            Assert.AreEqual(repository.Rules<TestClass1>().ElementAt(0), subject.Rules[2]);

            Assert.AreEqual(objectInstance, subject.ObjectInstance);
            Assert.AreEqual(di.Object, subject.DI);
        }
    }
}