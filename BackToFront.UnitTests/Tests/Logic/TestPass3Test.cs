using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions;
using BackToFront.UnitTests.Utilities;

using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Logic
{
    /// <summary>
    /// ToTest: and
    ///         If, is true, OR, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass3Test : Base.TestBase
    {
        public class TestPass3
        {
            public static SimpleViolation Violation = new SimpleViolation("Violation");

            static TestPass3()
            {
                Rules.Add<TestPass3>(rule => rule
                    .If(a => a.ThrowViolationSwitch1).IsTrue().Or(a => a.ThrowViolationSwitch2).IsTrue().ModelViolationIs(Violation));
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
            var v = switch1 || switch2 ? TestPass3.Violation : null;
            var subject = new TestPass3
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
