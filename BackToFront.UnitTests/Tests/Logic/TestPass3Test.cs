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
    public class TestPass3Test : Base.TestBase
    {
        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Test_Allcases(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? Utilities.TestClasses.TestPass3.Violation : null;
            var subject = new Utilities.TestClasses.TestPass3
            {
                ThrowViolationSwitch1 = switch1,
                ThrowViolationSwitch2 = switch2
            };

            // act
            var violation = subject.Validate();

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
