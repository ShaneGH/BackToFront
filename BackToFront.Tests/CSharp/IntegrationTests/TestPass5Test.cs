using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Validate;
using BackToFront.Tests.Utilities;

using NUnit.Framework;

namespace BackToFront.Tests.CSharp.IntegrationTests
{
    /// <summary>
    /// ToTest: Require And
    ///         If.....
    ///         Require, is false, OR, is false, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass5Test : Base.RulesRepositoryTestBase
    {
        public static TestViolation Violation = new TestViolation("Violation");
        public class TestPass5
        {
            public bool RequiredSwitch1 { get; set; }
            public bool RequiredSwitch2 { get; set; }
        }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Repository.AddRule<TestPass5>(rule => rule
                // pass through if
                    .If(a => a.RequiredSwitch1 || !a.RequiredSwitch1)

                    .RequireThat(a => a.RequiredSwitch1 || a.RequiredSwitch2).WithModelViolation(() => Violation));

        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Require_Or(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? null : Violation;
            var subject = new TestPass5
            {
                RequiredSwitch1 = switch1,
                RequiredSwitch2 = switch2
            };

            // act
            var violation = subject.Validate(Repository).FirstViolation;

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
