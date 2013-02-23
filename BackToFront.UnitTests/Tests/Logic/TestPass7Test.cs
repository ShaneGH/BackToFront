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
    /// ToTest: Require And
    ///         If.....
    ///         Require, is false, OR, is false, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass7Test : Base.TestBase
    {
        public static SimpleViolation Violation1 = new SimpleViolation("Violation");
        public static SimpleViolation Violation2 = new SimpleViolation("Violation");

        public class TestEntity
        {

            static TestEntity()
            {
                Rules.Add<TestEntity>(rule => rule
                    // pass through if
                    .If(a => a.ThrowViolationSwitch1).IsTrue().Or(a => a.ThrowViolationSwitch1).IsFalse().Then(subRule =>
                    {
                        subRule.RequireThat(a => a.RequiredSwitch1).IsTrue().OrModelViolationIs(Violation1);
                        subRule.RequireThat(a => a.RequiredSwitch2).IsTrue().OrModelViolationIs(Violation1);
                    }));

                Rules.Add<TestEntity>(rule => rule
                    .If(a => a.ThrowViolationSwitch1).IsTrue().Or(a => a.ThrowViolationSwitch2).IsTrue().ModelViolationIs(Violation2));
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
        public void If_Or_Then_if_allVio9lations(Tuple<bool, bool, bool, bool> input)
        {
            bool throw1 = input.Item1,
                throw2 = input.Item2,
                required1 = input.Item3,
                required2 = input.Item4;

            // arrange
            var v = new List<SimpleViolation>();
            if (!required1)
                v.Add(Violation1);
            if (!required2)
                v.Add(Violation1);
            if (throw1 || throw2)
                v.Add(Violation2);

            var subject = new TestEntity
            {
                RequiredSwitch1 = required1,
                RequiredSwitch2 = required2,
                ThrowViolationSwitch1 = throw1,
                ThrowViolationSwitch2 = throw2
            };

            // act
            var violation = subject.ValidateAllRules();

            // assert
            Assert.AreEqual(v.Count, violation.Count());
            for (int i = 0; i < v.Count; i++)
                Assert.AreEqual(v[i], violation.ElementAt(i));
        }

        [Test]
        [TestCaseSource("GetData")]
        public void If_Or_Then_if_oneViolation(Tuple<bool, bool, bool, bool> input)
        {
            bool throw1 = input.Item1,
                throw2 = input.Item2,
                required1 = input.Item3,
                required2 = input.Item4;

            // arrange
            SimpleViolation v = null;
            if (!required1)
                v = Violation1;
            else if (!required2)
                v = Violation1;
            else if (throw1 || throw2)
                v = Violation2;

            var subject = new TestEntity
            {
                RequiredSwitch1 = required1,
                RequiredSwitch2 = required2,
                ThrowViolationSwitch1 = throw1,
                ThrowViolationSwitch2 = throw2
            };

            // act
            var violation = subject.Validate();

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
