﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Validate;
using BackToFront.Tests.Utilities;

using NUnit.Framework;

namespace BackToFront.Tests.IntegrationTests
{
    /// <summary>
    /// ToTest: Require And
    ///         If.....
    ///         Require, is false, AND, is false, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass4Test_1 : Base.TestBase
    {
        public class TestPass4
        {
            public static SimpleViolation Violation = new SimpleViolation("Violation");

            static TestPass4()
            {
                Rules<TestPass4>.AddRule(rule => rule
                    .RequireThat(a => !a.ThrowViolationSwitch1 && !a.ThrowViolationSwitch2).WithModelViolation(Violation));
            }

            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Require_And(bool switch1, bool switch2)
        {
            // arrange
            var v = switch1 || switch2 ? TestPass4.Violation : null;
            var subject = new TestPass4
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