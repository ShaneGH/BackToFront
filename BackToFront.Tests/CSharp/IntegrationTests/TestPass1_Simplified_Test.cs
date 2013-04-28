using BackToFront.Enum;
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
    public class TestPass1_Simplified_Test : Base.TestBase
    {
        public class TestPass1
        {
            public static TestViolation Violation1 = new TestViolation("Violation1");
            public static TestViolation Violation2 = new TestViolation("Violation2");

            static TestPass1()
            {
                Rules<TestPass1>.AddRule(rule => rule
                    .RequireThat(a => !a.ThrowViolation1).WithModelViolation(() => Violation1));

                Rules<TestPass1>.AddRule(rule => rule
                    .RequireThat(a => !a.ThrowViolation2).WithModelViolation(() => Violation2));
            }

            public bool ThrowViolation1 { get; set; }
            public bool ThrowViolation2 { get; set; }
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
            var violation1 = subject.Validate().AllViolations;

            // assert
            Assert.AreEqual(2, violation1.Count());
            Assert.AreEqual(TestPass1.Violation1, violation1.ElementAt(0));
            Assert.AreEqual(TestPass1.Violation2, violation1.ElementAt(1));
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
            Assert.AreEqual(0, subject.Validate()
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
            Assert.AreEqual(1, subject.Validate()
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
            Assert.AreEqual(0, subject.Validate()
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
            Assert.AreEqual(1, subject.Validate()
                .WithMockedParameter(a => a.ThrowViolation1, false, BackToFront.Enum.MockBehavior.SetOnly)
                .AllViolations.Count());

            Assert.IsFalse(subject.ThrowViolation1);
        }
    }
}
