﻿using System;
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
    /// ToTest: and
    ///         If, is true, OR, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass3Test : Base.RulesRepositoryTestBase
    {
        public static TestViolation Violation = new TestViolation("Violation");
        public class TestClass
        {
            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
        }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Repository.AddRule<TestClass>(rule => rule
                    .If(a => a.ThrowViolationSwitch1 || a.ThrowViolationSwitch2).RequirementFailed.WithModelViolation(() => Violation));

        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void If_Or(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? Violation : null;
            var subject = new TestClass
            {
                ThrowViolationSwitch1 = switch1,
                ThrowViolationSwitch2 = switch2
            };

            // act
            var violation = subject.Validate(Repository).FirstViolation;

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
