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

    /// <summary>
    /// ToTest: and
    ///         If, is true, and, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass2Test : Base.TestBase
    {
        public class TestPass2
        {
            public static SimpleViolation Violation = new SimpleViolation("Violation");

            static TestPass2()
            {
                Rules.Add<TestPass2>(rule => rule
                    .If(a => a.ThrowViolationSwitch1 && a.ThrowViolationSwitch2).RequirementFailed.OrModelViolationIs(Violation));
            }

            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void If_And(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 && switch2 ? TestPass2.Violation : null;
            var subject = new TestPass2
            {
                ThrowViolationSwitch1 = switch1,
                ThrowViolationSwitch2 = switch2
            };

            // act
            var violation = subject.Validate().FirstViolation;

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
