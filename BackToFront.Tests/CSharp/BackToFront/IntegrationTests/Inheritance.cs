﻿
using BackToFront.Tests.Utilities;
using BackToFront.Validate;
using NUnit.Framework;

namespace BackToFront.Tests.CSharp.IntegrationTests
{
    /// <summary>
    /// ToTest: Require And
    ///         If.....
    ///         Require, is false, AND, is false, model violation is
    /// </summary>
    [TestFixture]
    public class Inheritance : Base.RulesRepositoryTestBase
    {
        public static TestViolation Violation = new TestViolation("Violation");
        public class TestPass4
        {
            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
        }

        public class TestPass4Child : TestPass4 { }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Repository.AddRule<TestPass4>(rule => rule
                .RequireThat(a => !a.ThrowViolationSwitch1 && !a.ThrowViolationSwitch2).WithModelViolation(() => Violation));
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Require_And(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? Violation : null;
            var subject = new TestPass4Child
            {
                ThrowViolationSwitch1 = switch1,
                ThrowViolationSwitch2 = switch2
            };

            // act
            var violation = subject.Validate(Repository).FirstViolation;

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
