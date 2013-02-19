using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions;

using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Logic
{
    [TestFixture]
    public class TestPass1Test : Base.TestBase
    {
        [Test]
        public void Test_NoViolations()
        {
            // arrange
            var subject = new Utilities.TestClasses.TestPass1
            {
                ThrowViolation1 = false,
                ThrowViolation2 = false
            };

            // act
            var violation = subject.ValidateAllRules();

            // assert
            Assert.AreEqual(0, violation.Count());
        }

        [Test]
        public void Test_1Violation()
        {
            // arrange
            var subject = new Utilities.TestClasses.TestPass1
            {
                ThrowViolation1 = false,
                ThrowViolation2 = true
            };

            // act
            var violation = subject.ValidateAllRules();

            // assert
            Assert.AreEqual(1, violation.Count());
            Assert.AreEqual(Utilities.TestClasses.TestPass1.Violation2, violation.ElementAt(0));
        }

        [Test]
        public void Test_2Violations()
        {
            // arrange
            var subject = new Utilities.TestClasses.TestPass1
            {
                ThrowViolation1 = true,
                ThrowViolation2 = true
            };

            // act
            var violation = subject.ValidateAllRules();

            // assert
            Assert.AreEqual(2, violation.Count());
            Assert.AreEqual(Utilities.TestClasses.TestPass1.Violation1, violation.ElementAt(0));
            Assert.AreEqual(Utilities.TestClasses.TestPass1.Violation2, violation.ElementAt(1));
        }
    }
}
