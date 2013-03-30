using System;
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
    ///         Require, is false, OR, is false, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass6Test : Base.TestBase
    {
        public class TestEntity
        {
            public static SimpleViolation Violation = new SimpleViolation("Violation");

            static TestEntity()
            {
                Rules<TestEntity>.AddRule(rule => rule
                    .If(a => a.ThrowViolationSwitch1 || a.ThrowViolationSwitch2)

                    .RequireThat(a => a.RequiredSwitch1 && a.RequiredSwitch2).WithModelViolation(Violation));
            }

            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; }
            public bool RequiredSwitch1 { get; set; }
            public bool RequiredSwitch2 { get; set; }
        }

        public static IEnumerable<Tuple<bool, bool, bool, bool>> GetData()
        {
            for (var i = 0; i < 16; i++)
            {
                var bits = new System.Collections.BitArray(new[] { (byte)i });
                yield return new Tuple<bool, bool, bool, bool>(bits[3], bits[2], bits[1], bits[0]);
            }
        }

        [Test]
        [TestCaseSource("GetData")]
        public void If_Or_Require_And(Tuple<bool, bool, bool, bool> input)
        {
            bool throw1 = input.Item1,
                throw2 = input.Item2,
                required1 = input.Item3,
                required2 = input.Item4;

            // arrange
            var v = (throw1 || throw2) && !(required1 && required2) ? TestEntity.Violation : null;
            var subject = new TestEntity
            {
                RequiredSwitch1 = required1,
                RequiredSwitch2 = required2,
                ThrowViolationSwitch1 = throw1,
                ThrowViolationSwitch2 = throw2
            };

            // act
            var violation = subject.Validate().FirstViolation;

            // assert
            Assert.AreEqual(v, violation, "(" + throw1.ToString() + " || " + throw2.ToString() + ")" + " && " + "(" + required1.ToString() + " && " + required2.ToString() + ")");
        }
    }
}
