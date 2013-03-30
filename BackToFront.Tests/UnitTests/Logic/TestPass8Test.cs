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
    /// ToTest: If, else if, else
    /// </summary>
    [TestFixture]
    public class TestPass8Test : Base.TestBase
    {
        public static SimpleViolation Violation1 = new SimpleViolation("Violation1");
        public static SimpleViolation Violation2 = new SimpleViolation("Violation2");
        public static SimpleViolation Violation3= new SimpleViolation("Violation3");

        public class TestEntity
        {

            static TestEntity()
            {
                Rules<TestEntity>.AddRule(rule => rule
                    .If(a => a.ThrowViolationSwitch1).RequirementFailed.OrModelViolationIs(Violation1)
                    .ElseIf(a => a.ThrowViolationSwitch2).RequirementFailed.OrModelViolationIs(Violation2)
                    .Else.RequirementFailed.OrModelViolationIs(Violation3));
            }

            public bool ThrowViolationSwitch1 { get; set; }
            public bool ThrowViolationSwitch2 { get; set; } 
        }

        public static IEnumerable<Tuple<bool, bool>> GetData()
        {
            for (var i = 0; i < 4; i++)
            {
                var bits = new System.Collections.BitArray(new[] { (byte)i });
                yield return new Tuple<bool, bool>(bits[1], bits[0]);
            }
        }

        [Test]
        [TestCaseSource("GetData")]
        public void If_elseIf(Tuple<bool, bool> input)
        {
            bool
                throw1 = input.Item1,
                throw2 = input.Item2;

            // arrange
            SimpleViolation v = null;
            if (throw1)
                v = Violation1;
            else if (throw2)
                v = Violation2;
            else
                v = Violation3;

            var subject = new TestEntity
            {
                ThrowViolationSwitch1 = throw1,
                ThrowViolationSwitch2 = throw2
            };

            // act
            var violation = subject.Validate().AllViolations;

            // assert
            Assert.AreEqual(1, violation.Count());
            Assert.AreEqual(v, violation.ElementAt(0));
        }
    }
}
