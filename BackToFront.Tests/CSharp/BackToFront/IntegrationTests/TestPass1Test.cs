﻿using BackToFront.Enum;
using BackToFront.Tests.Utilities;
using BackToFront.Validate;
using NUnit.Framework;
using System.Linq;

namespace BackToFront.Tests.CSharp.IntegrationTests
{
    /// <summary>
    /// ToTest: multiple rules
    ///         If, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass1Test : Base.RulesRepositoryTestBase
    {
        public static TestViolation Violation1 = new TestViolation("Violation");
        public static TestViolation Violation2 = new TestViolation("Violation");
        public class TestPass1
        {
            public bool ThrowViolation1 { get; set; }
            public bool ThrowViolation2 { get; set; }
        }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Repository.AddRule<TestPass1>(rule => rule
                    .If(a => a.ThrowViolation1).RequirementFailed.WithModelViolation(() => Violation1));

            Repository.AddRule<TestPass1>(rule => rule
                .If(a => a.ThrowViolation2).RequirementFailed.WithModelViolation(() => Violation2));

        }

        [Test]
        public void Test_If_NoViolations()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = false,
                ThrowViolation2 = false
            };

            // act
            var violation = subject.Validate(Repository).AllViolations;

            // assert
            Assert.AreEqual(0, violation.Count());
        }

        [Test]
        public void Test_If_1Violation()
        {
            TestPass1 subject = new TestPass1
                {
                    ThrowViolation1 = false,
                    ThrowViolation2 = true
                };

            // act
            var violation = subject.Validate(Repository).AllViolations;

            // assert
            Assert.AreEqual(1, violation.Count());
            Assert.AreEqual(Violation2, violation.ElementAt(0));
        }

        [Test]
        public void Test_If_2Violations()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = true,
                ThrowViolation2 = true
            };

            // act
            var violation = subject.Validate(Repository).AllViolations;

            // assert
            Assert.AreEqual(2, violation.Count());
            Assert.AreEqual(Violation1, violation.ElementAt(0));
            Assert.AreEqual(Violation2, violation.ElementAt(1));
        }

        [Test]
        public void Test_Mocking()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = false,
                ThrowViolation2 = false
            };

            // act
            // assert
            Assert.AreEqual(0, subject.Validate(Repository).AllViolations.Count());
            Assert.AreEqual(2, subject.Validate(Repository)
                .WithMockedParameter(a => a.ThrowViolation1, true)
                .WithMockedParameter(a => a.ThrowViolation2, true)
                .AllViolations.Count());
        }

        [Test]
        public void Test_Mocking_MockOnly()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = true,
                ThrowViolation2 = false
            };

            // act
            // assert
            Assert.AreEqual(0, subject.Validate(Repository)
                .WithMockedParameter(a => a.ThrowViolation1, false, MockBehavior.MockOnly)
                .AllViolations.Count());

            Assert.IsTrue(subject.ThrowViolation1);
        }

        [Test]
        public void Test_Mocking_MockAndSet_Pass()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = true,
                ThrowViolation2 = false
            };

            // act
            // assert
            Assert.AreEqual(0, subject.Validate(Repository)
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Enum.MockBehavior.MockAndSet)
                .AllViolations.Count());

            Assert.IsFalse(subject.ThrowViolation1);
        }

        [Test]
        public void Test_Mocking_MockAndSet_Fail()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = true,
                ThrowViolation2 = true
            };

            // act
            // assert
            Assert.AreEqual(1, subject.Validate(Repository)
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Enum.MockBehavior.MockAndSet)
                .AllViolations.Count());

            Assert.IsTrue(subject.ThrowViolation1);
        }

        [Test]
        public void Test_Mocking_SetOnly_Pass()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = false,
                ThrowViolation2 = false
            };

            // act
            // assert
            Assert.AreEqual(0, subject.Validate(Repository)
                .WithMockedParameter(a => a.ThrowViolation1, true, BackToFront.Enum.MockBehavior.SetOnly)
                .AllViolations.Count());

            Assert.IsTrue(subject.ThrowViolation1);
        }

        [Test]
        public void Test_Mocking_SetOnly_Fail()
        {
            // arrange
            var subject = new TestPass1
            {
                ThrowViolation1 = false,
                ThrowViolation2 = true
            };

            // act
            // assert
            Assert.AreEqual(1, subject.Validate(Repository)
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Enum.MockBehavior.SetOnly)
                .AllViolations.Count());

            Assert.IsFalse(subject.ThrowViolation1);
        }
    }
}
