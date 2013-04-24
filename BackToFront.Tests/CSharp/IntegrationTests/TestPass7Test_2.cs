using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Validate;
using BackToFront.Tests.Utilities;

using NUnit.Framework;
using System.Runtime.Serialization;
using BackToFront.Meta;
using System.IO;
using System.Runtime.Serialization.Json;

namespace BackToFront.Tests.IntegrationTests
{
    /// <summary>
    /// ToTest: Require And
    ///         If.....
    ///         Require, is false, OR, is false, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass7Test_2 : Base.TestBase
    {
        public static TestViolation Violation1 = new TestViolation("Violation");
        public static TestViolation Violation2 = new TestViolation("Violation");

        public class TestEntity
        {

            static TestEntity()
            {
                Rules<TestEntity>.AddRule(rule => rule
                    .If(a => a.ContinueSwitch).Then(subRule =>
                    {
                        subRule.If(a => !a.RequiredSwitch1).RequirementFailed.WithModelViolation(() => Violation1);
                        subRule.If(a => !a.RequiredSwitch2).RequirementFailed.WithModelViolation(() => Violation1);
                    }));
            }

            public bool ContinueSwitch { get; set; }
            public bool RequiredSwitch1 { get; set; }
            public bool RequiredSwitch2 { get; set; }
        }

        public static IEnumerable<Tuple<bool, bool, bool>> GetData()
        {
            for (var i = 0; i < 8; i++)
            {
                var bits = new System.Collections.BitArray(new[] { (byte)i });
                yield return new Tuple<bool, bool, bool>(bits[2], bits[1], bits[0]);
            }
        }

        [Test]
        [TestCaseSource("GetData")]
        public void If_Or_Then_if_allVio9lations(Tuple<bool, bool, bool> input)
        {
            bool continueOn = input.Item1,
                required1 = input.Item2,
                required2 = input.Item3;

            // arrange
            var v = new List<TestViolation>();
            if (continueOn && !required1)
                v.Add(Violation1);
            if (continueOn && !required2)
                v.Add(Violation1);

            var subject = new TestEntity
            {
                RequiredSwitch1 = required1,
                RequiredSwitch2 = required2,
                ContinueSwitch = continueOn
            };

            // act
            var violation = subject.Validate().AllViolations;

            // assert
            Assert.AreEqual(v.Count, violation.Count());
            for (int i = 0; i < v.Count; i++)
                Assert.AreEqual(v[i], violation.ElementAt(i));
        }

        [Test]
        [TestCaseSource("GetData")]
        public void If_Or_Then_if_oneViolation(Tuple<bool, bool, bool> input)
        {
            bool continueOn = input.Item1,
                required1 = input.Item2,
                required2 = input.Item3;

            // arrange
            TestViolation v = null;
            if (continueOn && !required1)
                v = Violation1;
            else if (continueOn && !required2)
                v = Violation1;

            var subject = new TestEntity
            {
                RequiredSwitch1 = required1,
                RequiredSwitch2 = required2,
                ContinueSwitch = continueOn
            };

            // act
            var violation = subject.Validate().FirstViolation;

            // assert
            Assert.AreEqual(v, violation);
        }
    }
}
