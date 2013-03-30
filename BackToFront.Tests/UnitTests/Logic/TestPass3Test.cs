using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Validate;
using BackToFront.Tests.Utilities;

using NUnit.Framework;

namespace BackToFront.Tests.UnitTests.Logic
{
    /// <summary>
    /// ToTest: and
    ///         If, is true, OR, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass3Test : Base.TestBase
    {
        public class TestClass
        {
            public static SimpleViolation Violation = new SimpleViolation("Violation");

            static TestClass()
            {
                Rules<TestClass>.AddRule(rule => rule
                    .If(a => a.ThrowViolationSwitch1 || a.ThrowViolationSwitch2).RequirementFailed.OrModelViolationIs(Violation));
            }

            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void If_Or(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? TestClass.Violation : null;
            var subject = new TestClass
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
