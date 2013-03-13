using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Validate;
using BackToFront.UnitTests.Utilities;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Logic
{
    public class TestPass1
    {
        public static SimpleViolation Violation1 = new SimpleViolation("Violation");
        public static SimpleViolation Violation2 = new SimpleViolation("Violation");

        static TestPass1()
        {
            Rules.Add<TestPass1>(rule => rule
                .If(a => a.ThrowViolation1).ModelViolationIs(Violation1));

            Rules.Add<TestPass1>(rule => rule
                .If(a => a.ThrowViolation2).ModelViolationIs(Violation2));
        }

        public bool ThrowViolation1 { get; set; }
        public bool ThrowViolation2 { get; set; }
    }

    /// <summary>
    /// ToTest: multiple rules
    ///         If, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass1Test : Base.TestBase
    {
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
            var violation = subject.Validate().AllViolations;

            // assert
            Assert.AreEqual(0, violation.Count());
        }

        [Test]
        public void Test_If_1Violation()
        {
            TestPass1 subject =  new TestPass1
                {
                    ThrowViolation1 = false,
                    ThrowViolation2 = true
                };

            // act
            var violation = subject.Validate().AllViolations;

            // assert
            Assert.AreEqual(1, violation.Count());
            Assert.AreEqual(TestPass1.Violation2, violation.ElementAt(0));
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
            var violation = subject.Validate().AllViolations;

            // assert
            Assert.AreEqual(2, violation.Count());
            Assert.AreEqual(TestPass1.Violation1, violation.ElementAt(0));
            Assert.AreEqual(TestPass1.Violation2, violation.ElementAt(1));
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
            Assert.AreEqual(0, subject.Validate().AllViolations.Count());
            Assert.AreEqual(2, subject.Validate()
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
            Assert.AreEqual(0, subject.Validate()
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Utils.MockBehavior.MockOnly)
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
            Assert.AreEqual(0, subject.Validate()
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Utils.MockBehavior.MockAndSet)
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
            Assert.AreEqual(1, subject.Validate()
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Utils.MockBehavior.MockAndSet)
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
            Assert.AreEqual(0, subject.Validate()
                .WithMockedParameter(a => a.ThrowViolation1, true, BackToFront.Utils.MockBehavior.SetOnly)
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
            Assert.AreEqual(1, subject.Validate()
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Utils.MockBehavior.SetOnly)
                .AllViolations.Count());

            Assert.IsFalse(subject.ThrowViolation1);
        }
    }
}
