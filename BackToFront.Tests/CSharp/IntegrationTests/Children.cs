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
    ///         Require, is false, AND, is false, model violation is
    /// </summary>
    [TestFixture]
    public class Children : Base.TestBase
    {
        public class ParentClass
        {
            public TestPass4 Child { get; set; }
        }

        public class TestPass4
        {
            public static TestViolation Violation = new TestViolation("Violation");

            static TestPass4()
            {
                Rules<TestPass4>.AddRule(rule => rule
                    .RequireThat(a => !a.ThrowViolationSwitch1 && !a.ThrowViolationSwitch2).WithModelViolation(() => Violation));
            }

            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
        }

        public class TestPass4Child : TestPass4 { }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Require_And(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? TestPass4Child.Violation : null;
            var subject = new ParentClass
            {
                Child = new TestPass4Child
                {
                    ThrowViolationSwitch1 = switch1,
                    ThrowViolationSwitch2 = switch2
                }
            };

            // act
            var violation1 = subject.Validate().FirstViolation;
            var violation2 = subject.Validate().ValidateMember(a => a.Child).FirstViolation;

            // assert
            Assert.IsNull(violation1);
            Assert.AreEqual(v, violation2);
            if (switch1 || switch2)
                Assert.AreEqual(subject.Child, violation2.ViolatedEntity);
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Require_And_WithMocks(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? TestPass4Child.Violation : null;
            var subject = new ParentClass
            {
                Child = new TestPass4Child()
            };

            // act
            var violation1 = subject.Validate().FirstViolation;
            var violation2 = subject.Validate()
                .WithMockedParameter(a => a.Child.ThrowViolationSwitch1, switch1)
                .WithMockedParameter(a => a.Child.ThrowViolationSwitch2, switch2)
                .ValidateMember(a => a.Child).FirstViolation;

            // assert
            Assert.IsNull(violation1);
            Assert.AreEqual(v, violation2);
            if (switch1 || switch2)
                Assert.AreEqual(subject.Child, violation2.ViolatedEntity);
        }
    }
}
